using System.Net;
using System.Security.Claims;

using RestSharp;

using RealmHelper.Application;
using RealmHelper.Application.Api;
using RealmHelper.Application.Authentication;
using RealmHelper.Application.Models.Minecraft;
using RealmHelper.Application.Models.Minecraft.Requests;
using RealmHelper.Application.Models.Minecraft.Responses;
using RealmHelper.Infrastructure.Authentication;
using RealmHelper.Infrastructure.Http;

namespace RealmHelper.Infrastructure.Api;

public class BedrockRealmApi : IBedrockRealmApi
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

    public BedrockRealmApi(ClaimsPrincipal user)
    {
        var xstsToken = user.FindFirst(AuthClaims.BedrockXstsToken)!.Value;
        var userHash = user.FindFirst(AuthClaims.XboxLiveUserHash)!.Value;

        _restClient = new RestClient(Endpoint, opts =>
                opts.Authenticator = new XboxLiveAuthenticator(xstsToken, userHash)
            )
            .AddDefaultHeader(KnownHeaders.Accept, "*/*")
            .AddDefaultHeader(KnownHeaders.UserAgent, "MCPE/UWP")
            .AddDefaultHeader(AdditionalHeaders.ClientVersion, MinecraftConstants.Version);
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

    public async Task<ArchiveResponse> RequestDownloadInfo(long realmId, int slotId, string? backupId = null,
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

        var response = await _restClient.GetAsync<BedrockArchiveDownloadResponse>(request, cancellationToken);

        return response!;
    }

    public async Task<ArchiveResponse> RequestUploadInfo(long realmId, int slotId,
        CancellationToken cancellationToken = default)
    {
        var request = new RestRequest(
            UploadPath.Replace("{WORLD}", realmId.ToString()).Replace("{SLOT}", slotId.ToString())
        );

        var response = await _restClient.GetAsync<BedrockArchiveUploadResponse>(request, cancellationToken);

        return response!;
    }

    public Task RestoreBackupAsync(long realmId, string backupId, CancellationToken cancellationToken = default)
    {
        var request = new RestRequest(BackupsPath.Replace("{WORLD}", realmId.ToString()), Method.Put)
            .AddQueryParameter("backupId", backupId)
            .AddQueryParameter("clientSupportsRetries", true);

        return _restClient.PutAsync(request, cancellationToken);
    }

    public Task InvitePlayerAsync(long realmId, string player, CancellationToken cancellationToken = default) =>
        UpdateInvitesAsync(realmId, player, "ADD", cancellationToken);

    public Task UninvitePlayerAsync(long realmId, string player, CancellationToken cancellationToken = default) =>
        UpdateInvitesAsync(realmId, player, "REMOVE", cancellationToken);

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

    public async Task<PlayerActivitiesResponse<BedrockPlayerResponse, BedrockPlayerActivityResponse>>
        GetPlayerActivitiesAsync(
            CancellationToken cancellationToken = default)
    {
        var request = new RestRequest(LivePlayersPath);
        var response =
            await _restClient.GetAsync<PlayerActivitiesResponse<BedrockPlayerResponse, BedrockPlayerActivityResponse>>(
                request, cancellationToken);

        return response!;
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
}

public class JavaRealmApi : IJavaRealmApi
{
    private const string Endpoint = "https://pc.realms.minecraft.net";

    private const string WorldsPath = "/worlds";
    private const string WorldPath = WorldsPath + "/{WORLD}";
    private const string OpenPath = WorldPath + "/open";
    private const string ClosePath = WorldPath + "/close";
    private const string ResetPath = WorldPath + "/reset";
    private const string BackupsPath = WorldPath + "/backups";
    private const string UploadPath = BackupsPath + "/upload";
    private const string SlotPath = WorldPath + "/slot/{SLOT}";
    private const string DownloadPath = SlotPath + "/download";

    private const string ActivitiesPath = "/activities";
    private const string LivePlayerListPath = ActivitiesPath + "/livePlayerList";

    private const string InvitesPath = "/invites/{WORLD}";
    private const string InvitePath = InvitesPath + "/invite/{PLAYER}";
    
    private const string OpsPath = "/ops/{WORLD}";
    private const string OpPath = OpsPath + "/{PLAYER}";

    private readonly RestClient _restClient;

    public JavaRealmApi(ClaimsPrincipal user)
    {
        var username = user.FindFirst(AuthClaims.JavaUser)?.Value;
        var uuid = user.FindFirst(AuthClaims.JavaUuid)?.Value;
        var token = user.FindFirst(AuthClaims.JavaAccessToken)?.Value;
        var version = MinecraftConstants.Version;
        var sid = $"token:{token}:{uuid}";

        _restClient = new RestClient(Endpoint, opts =>
        {
            opts.CookieContainer = new CookieContainer();
            opts.CookieContainer.Add(new Cookie("sid", sid, null, "pc.realms.minecraft.net"));
            opts.CookieContainer.Add(new Cookie("user", username, null, "pc.realms.minecraft.net"));
            opts.CookieContainer.Add(new Cookie("version", version, null, "pc.realms.minecraft.net"));
        });
        
        _restClient.AcceptedContentTypes = new[] { "*/*" };
    }

    public Task<RealmsResponse<JavaRealmResponse>> GetRealmsAsync(CancellationToken cancellationToken = default) =>
        _restClient.GetJsonAsync<RealmsResponse<JavaRealmResponse>>(WorldsPath, cancellationToken)!;

    public Task<JavaRealmResponse> GetRealmAsync(long realmId, CancellationToken cancellationToken = default) =>
        _restClient.GetJsonAsync<JavaRealmResponse>(WorldPath.Replace("{WORLD}", realmId.ToString()), cancellationToken)!;

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
        var request = new RestRequest(ResetPath.Replace("{WORLD}", realmId.ToString()), Method.Post)
            .AddJsonBody(new
            {
                seed = Random.Shared.Next().ToString(),
                worldTemplateId = -1,
                levelType = 0,
                generateStructure = true
            });

        return _restClient.PostAsync(request, cancellationToken);
    }

    public Task<BackupsResponse> GetBackupsAsync(long realmId, CancellationToken cancellationToken = default) =>
        _restClient.GetJsonAsync<BackupsResponse>(BackupsPath.Replace("{WORLD}", realmId.ToString()), cancellationToken)!;

    public async Task<ArchiveResponse> RequestDownloadInfo(long realmId, int slotId, string? backupId,
        CancellationToken cancellationToken = default)
    {
        var response = await _restClient.GetJsonAsync<JavaArchiveDownloadResponse>(
            DownloadPath.Replace("{WORLD}", realmId.ToString()).Replace("{SLOT}", slotId.ToString()),
            cancellationToken
        );

        return response!;
    }

    public Task RestoreBackupAsync(long realmId, string backupId, CancellationToken cancellationToken = default)
    {
        var request = new RestRequest(BackupsPath.Replace("{WORLD}", realmId.ToString()), Method.Put)
            .AddQueryParameter("backupId", backupId);

        return _restClient.PutAsync(request, cancellationToken);
    }

    public async Task<ArchiveResponse> RequestUploadInfo(long realmId, int slotId,
        CancellationToken cancellationToken = default)
    {
        var request = new RestRequest(
            UploadPath.Replace("{WORLD}", realmId.ToString()).Replace("{SLOT}", slotId.ToString()),
            Method.Put
        );
        var response = await _restClient.PutAsync<JavaArchiveUploadResponse>(request, cancellationToken);

        return response!;
    }

    public Task InvitePlayerAsync(long realmId, string player, CancellationToken cancellationToken = default) =>
        _restClient.PutJsonAsync(InvitePath.Replace("{WORLD}", realmId.ToString()), new { name = player },
            cancellationToken);

    public Task UninvitePlayerAsync(long realmId, string player, CancellationToken cancellationToken = default)
    {
        var request = new RestRequest(
            InvitePath.Replace("{WORLD}", realmId.ToString()).Replace("{PLAYER}", player),
            Method.Delete
        );

        return _restClient.DeleteAsync(request, cancellationToken);
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
        CancellationToken cancellationToken = default) =>
        _restClient.PostJsonAsync(WorldPath.Replace("{WORLD}", realmId.ToString()), request,
            cancellationToken);

    public Task UpdateSlotAsync(long realmId, int slotId, SlotUpdateRequest<SlotOptionsDto> request,
        CancellationToken cancellationToken = default) =>
        Task.WhenAll(UpdateDescriptionAsync(realmId, request.Description, cancellationToken),
            UpdateSlotAsync(realmId, slotId, request.Options, cancellationToken));

    public Task UpdateSlotAsync(long realmId, int slotId, SlotOptionsDto options,
        CancellationToken cancellationToken = default) =>
        _restClient.PostJsonAsync(
            SlotPath.Replace("{WORLD}", realmId.ToString()).Replace("{SLOT}", slotId.ToString()),
            options,
            cancellationToken
        );

    public async Task<PlayerActivitiesResponse<string, JavaPlayerActivityResponse>> GetPlayerActivitiesAsync(
        CancellationToken cancellationToken = default)
    {
        var response = await _restClient.GetJsonAsync<JavaPlayerActivitiesResponse>(LivePlayerListPath, cancellationToken);

        return response!;
    }

    public Task OpAsync(long realmId, string player, CancellationToken cancellationToken = default)
    {
        var request = new RestRequest(
            OpPath.Replace("{WORLD}", realmId.ToString()).Replace("{PLAYER}", player),
            Method.Post
        );

        return _restClient.PostAsync(request, cancellationToken);
    }
    
    public Task DeopAsync(long realmId, string player, CancellationToken cancellationToken = default)
    {
        var request = new RestRequest(
            OpPath.Replace("{WORLD}", realmId.ToString()).Replace("{PLAYER}", player),
            Method.Delete
        );

        return _restClient.DeleteAsync(request, cancellationToken);
    }
}