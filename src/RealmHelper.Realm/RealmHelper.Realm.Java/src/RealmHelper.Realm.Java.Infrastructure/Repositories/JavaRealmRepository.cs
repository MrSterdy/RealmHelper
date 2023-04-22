using System.Net;
using System.Security.Claims;

using RestSharp;

using RealmHelper.Realm.Common.Abstractions.Models;
using RealmHelper.Realm.Common.Application.Models.Requests;
using RealmHelper.Realm.Common.Application.Models.Responses;
using RealmHelper.Realm.Java.Application.Models.Responses;
using RealmHelper.Realm.Java.Application.Repositories;
using RealmHelper.Realm.Java.Infrastructure.Authentication;

namespace RealmHelper.Realm.Java.Infrastructure.Repositories;

public class JavaRealmRepository : IJavaRealmRepository
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

    public JavaRealmRepository(ClaimsPrincipal user)
    {
        var sid = user.FindFirst(AuthClaims.Sid)!.Value;
        var username = user.FindFirst(AuthClaims.User)!.Value;
        var version = user.FindFirst(AuthClaims.Version)!.Value;

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