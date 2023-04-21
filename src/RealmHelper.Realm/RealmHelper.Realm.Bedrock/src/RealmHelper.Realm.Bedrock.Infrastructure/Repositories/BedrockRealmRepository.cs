using System.Security.Claims;

using Microsoft.Net.Http.Headers;

using RestSharp;

using RealmHelper.Realm.Bedrock.Abstractions.Models;
using RealmHelper.Realm.Bedrock.Application;
using RealmHelper.Realm.Bedrock.Application.Models.Requests;
using RealmHelper.Realm.Bedrock.Application.Models.Responses;
using RealmHelper.Realm.Bedrock.Application.Repositories;
using RealmHelper.Realm.Bedrock.Application.Types;
using RealmHelper.Realm.Bedrock.Infrastructure.Authentication;
using RealmHelper.Realm.Bedrock.Infrastructure.Http;
using RealmHelper.Realm.Common.Abstractions.Models;
using RealmHelper.Realm.Common.Application.Models.Requests;
using RealmHelper.Realm.Common.Application.Models.Responses;

namespace RealmHelper.Realm.Bedrock.Infrastructure.Repositories;

public sealed class BedrockRealmRepository : IBedrockRealmRepository, IDisposable
{
    private const string Endpoint = "https://pocket.realms.minecraft.net";

    private const string WorldsPath = "/worlds";
    private const string WorldPath = WorldsPath + "/{WORLD}";
    private const string BackupsPath = WorldPath + "/backups";
    private const string ConfigurationPath = WorldPath + "/configuration";
    private const string BlocklistPath = WorldPath + "/blocklist";
    private const string BlockPath = BlocklistPath + "/{PLAYER}";
    private const string UserPermissionPath = WorldPath + "/userPermission";
    private const string DefaultPermissionPath = WorldPath + "/defaultPermission";
    private const string SlotPath = WorldPath + "/slot/{SLOT}";
    private const string ClosePath = WorldPath + "/close";
    private const string OpenPath = WorldPath + "/open";
    private const string ResetPath = WorldPath + "/reset";

    private const string LivePlayersPath = "/activities/live/players";

    private const string ArchivePath = "/archive";
    private const string UploadPath = ArchivePath + "/upload/world/{WORLD}/{SLOT}";
    private const string DownloadPath = ArchivePath + "/download/world/{WORLD}/{SLOT}";
    private const string DownloadLatestPath = DownloadPath + "/latest";
    private const string DownloadBackupPath = DownloadPath + "/{BACKUP}";

    private const string InvitesPath = "/invites/{WORLD}";
    private const string InvitesUpdatePath = InvitesPath + "/invite/update";

    private const string LinksPath = "/links/v1";

    private readonly IRestClient _restClient;

    public BedrockRealmRepository(ClaimsPrincipal user)
    {
        var xstsToken = user.FindFirst(AuthClaims.XstsToken)!.Value;
        var userHash = user.FindFirst(AuthClaims.UserHash)!.Value;

        _restClient = new RestClient(Endpoint, opts =>
                opts.Authenticator = new XboxLiveAuthenticator(xstsToken, userHash)
            )
            .AddDefaultHeader(KnownHeaders.Accept, "*/*")
            .AddDefaultHeader(HeaderNames.UserAgent, "MCPE/UWP")
            .AddDefaultHeader(MinecraftHeaders.ClientVersion, MinecraftConstants.Version);
    }
    
    public Task<RealmsResponse<BedrockRealmResponse>> GetRealmsAsync(CancellationToken cancellationToken = default)
    {
        var request = new RestRequest(WorldsPath);

        return _restClient.GetAsync<RealmsResponse<BedrockRealmResponse>>(request, cancellationToken)!;
    }

    public Task<BedrockRealmResponse> GetRealmAsync(long realmId, CancellationToken cancellationToken = default)
    {
        var request = new RestRequest(WorldPath.Replace("{WORLD}", realmId.ToString()));

        return _restClient.GetAsync<BedrockRealmResponse>(request, cancellationToken)!;
    }

    public Task CloseRealmAsync(long realmId, CancellationToken cancellationToken = default)
    {
        var request = new RestRequest(ClosePath.Replace("{WORLD}", realmId.ToString()), Method.Put);

        return _restClient.PutAsync(request, cancellationToken);
    }

    public Task OpenRealmAsync(long realmId, CancellationToken cancellationToken = default)
    {
        var request = new RestRequest(OpenPath.Replace("{WORLD}", realmId.ToString()), Method.Put);

        return _restClient.PutAsync(request, cancellationToken);
    }

    public Task ResetRealmAsync(long realmId, CancellationToken cancellationToken = default)
    {
        var request = new RestRequest(ResetPath.Replace("{WORLD}", realmId.ToString()), Method.Put);

        return _restClient.PutAsync(request, cancellationToken);
    }

    public Task<BackupsResponse> GetBackupsAsync(long realmId, CancellationToken cancellationToken = default)
    {
        var request = new RestRequest(BackupsPath.Replace("{WORLD}", realmId.ToString()));

        return _restClient.GetAsync<BackupsResponse>(request, cancellationToken)!;
    }

    public async Task<ArchiveInfo> RequestDownloadInfo(long realmId, int slotId, string? backupId = null,
        CancellationToken cancellationToken = default)
    {
        var request = new RestRequest(backupId is null
            ? DownloadLatestPath
                .Replace("{WORLD}", realmId.ToString())
                .Replace("{SLOT}", slotId.ToString())
            : DownloadBackupPath
                .Replace("{WORLD}", realmId.ToString())
                .Replace("{SLOT}", slotId.ToString())
                .Replace("{BACKUP}", backupId)
        );

        var response = await _restClient.GetAsync<BedrockArchiveDownloadInfo>(request, cancellationToken);

        return new ArchiveInfo
        {
            Url = response!.DownloadUrl,
            Token = response.Token
        };
    }

    public async Task<ArchiveInfo> RequestUploadInfo(long realmId, int slotId,
        CancellationToken cancellationToken = default)
    {
        var request = new RestRequest(
            UploadPath.Replace("{WORLD}", realmId.ToString()).Replace("{SLOT}", slotId.ToString())
        );

        var response = await _restClient.GetAsync<BedrockArchiveUploadInfo>(request, cancellationToken);

        return new ArchiveInfo
        {
            Url = response!.UploadUrl,
            Token = response.Token
        };
    }

    public Task RestoreBackupAsync(long realmId, string backupId, CancellationToken cancellationToken = default)
    {
        var request = new RestRequest(BackupsPath.Replace("{WORLD}", realmId.ToString()), Method.Put)
            .AddQueryParameter("backupId", backupId)
            .AddQueryParameter("clientSupportsRetries", true);

        return _restClient.PutAsync(request, cancellationToken);
    }

    public Task InvitePlayerAsync(long realmId, string player, CancellationToken cancellationToken = default) =>
        UpdateInvitesAsync(realmId, player, InviteAction.Add, cancellationToken);

    public Task UninvitePlayerAsync(long realmId, string player, CancellationToken cancellationToken = default) =>
        UpdateInvitesAsync(realmId, player, InviteAction.Remove, cancellationToken);

    private Task UpdateInvitesAsync(long realmId, string player, string action,
        CancellationToken cancellationToken = default)
    {
        var request = new RestRequest(InvitesUpdatePath.Replace("{WORLD}", realmId.ToString()), Method.Put)
            .AddJsonBody(new { invites = new Dictionary<string, string> { { player, action } } });

        return _restClient.PutAsync(request, cancellationToken);
    }

    public Task ActivateSlotAsync(long realmId, int slotId, CancellationToken cancellationToken = default)
    {
        var request = new RestRequest(
            SlotPath.Replace("{WORLD}", realmId.ToString()).Replace("{SLOT}", slotId.ToString()),
            Method.Put
        );

        return _restClient.PutAsync(request, cancellationToken);
    }

    public Task UpdateDescriptionAsync(long realmId, DescriptionUpdateRequest request,
        CancellationToken cancellationToken = default)
    {
        var restRequest = new RestRequest(WorldPath.Replace("{WORLD}", realmId.ToString()), Method.Post)
            .AddJsonBody(request);

        return _restClient.PostAsync(restRequest, cancellationToken);
    }

    public Task UpdateSlotAsync(long realmId, int slotId, SlotUpdateRequest<BedrockSlotOptionsDto> request,
        CancellationToken cancellationToken = default)
    {
        var restRequest = new RestRequest(ConfigurationPath.Replace("{WORLD}", realmId.ToString()), Method.Post)
            .AddJsonBody(new { description = request.Description, options = request.Options });

        return _restClient.PostAsync(restRequest, cancellationToken);
    }

    public async Task<PlayerActivitiesResponse<BedrockPlayerResponse>> GetPlayerActivitiesAsync(
        CancellationToken cancellationToken = default)
    {
        var request = new RestRequest(LivePlayersPath);
        var response = await _restClient.GetAsync<LivePlayerListResponse>(request, cancellationToken);

        return new PlayerActivitiesResponse<BedrockPlayerResponse>
        {
            Servers = response!.Servers.Select(server =>
                new PlayerActivityResponse<BedrockPlayerResponse>
                {
                    RealmId = server.Id,
                    Players = server.Players
                }
            ).ToArray()
        };
    }

    public Task<string[]> GetBlockedPlayersAsync(long realmId, CancellationToken cancellationToken = default)
    {
        var request = new RestRequest(BlocklistPath.Replace("{WORLD}", realmId.ToString()));

        return _restClient.GetAsync<string[]>(request, cancellationToken)!;
    }

    public Task BlockPlayerAsync(long realmId, string xuid, CancellationToken cancellationToken = default)
    {
        var request = new RestRequest(
            BlockPath.Replace("{WORLD}", realmId.ToString()).Replace("{PLAYER}", xuid),
            Method.Post
        );

        return _restClient.PostAsync(request, cancellationToken);
    }

    public Task UnblockPlayerAsync(long realmId, string xuid, CancellationToken cancellationToken = default)
    {
        var request = new RestRequest(
            BlockPath.Replace("{WORLD}", realmId.ToString()).Replace("{PLAYER}", xuid),
            Method.Delete
        );

        return _restClient.DeleteAsync(request, cancellationToken);
    }

    public Task ChangePlayerPermissionAsync(long realmId, PlayerPermissionChangeRequest request,
        CancellationToken cancellationToken = default)
    {
        var restRequest = new RestRequest(UserPermissionPath.Replace("{WORLD}", realmId.ToString()), Method.Put)
            .AddJsonBody(request);

        return _restClient.PutAsync(restRequest, cancellationToken);
    }

    public Task ChangeDefaultPermissionAsync(long realmId, PermissionChangeRequest request,
        CancellationToken cancellationToken = default)
    {
        var restRequest = new RestRequest(DefaultPermissionPath.Replace("{WORLD}", realmId.ToString()), Method.Put)
            .AddJsonBody(request);

        return _restClient.PutAsync(restRequest, cancellationToken);
    }

    public Task<InviteLinkResponse[]> GetInviteCodesAsync(long realmId, CancellationToken cancellationToken = default)
    {
        var request = new RestRequest(LinksPath)
            .AddQueryParameter("worldId", realmId);

        return _restClient.GetAsync<InviteLinkResponse[]>(request, cancellationToken)!;
    }

    public Task<InviteLinkResponse> GenerateInviteCodeAsync(InviteLinkRequest request,
        CancellationToken cancellationToken = default)
    {
        var restRequest = new RestRequest(LinksPath, Method.Post)
            .AddJsonBody(request);

        return _restClient.PostAsync<InviteLinkResponse>(restRequest, cancellationToken)!;
    }

    public void Dispose() =>
        _restClient.Dispose();
}