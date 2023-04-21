using System.Net.Mime;
using System.Security.Claims;

using Microsoft.Extensions.Options;

using RestSharp;

using RealmHelper.Client.Infrastructure.Authentication;
using RealmHelper.Realm.Bedrock.Abstractions.Models;
using RealmHelper.Realm.Bedrock.Abstractions.Services;
using RealmHelper.Realm.Common.Abstractions.Models;
using RealmHelper.Realm.Java.Abstractions.Models;
using RealmHelper.Realm.Java.Abstractions.Services;

namespace RealmHelper.Client.Infrastructure.Services;

file static class Paths
{
    public const string Realms = "/realms";

    public const string Realm = "/realm/{REALM}";
    
    public const string Description = Realm + "/description";
    
    public const string Close = Realm + "/close";
    public const string Open = Realm + "/open";
    public const string Reset = Realm + "/reset";
    
    public const string Activity = Realm + "/activity";
    
    public const string Slot = Realm + "/slot/{SLOT}";
    
    public const string Backup = Slot + "/backup";
    public const string Backups = Realm + "/backups";
    public const string BackupDownload = Backup + "/download/{BACKUP}";
    public const string BackupUpload = Backup + "/upload/{BACKUP}";
    
    public const string Invite = Realm + "/invite/{PLAYER}";
}

public sealed class BedrockRealmService : IBedrockRealmService, IDisposable
{
    private const string BedrockPath = "/bedrock";
    
    private const string BlocklistPath = Paths.Realm + "/blocklist";
    private const string BlockPath = BlocklistPath + "/{PLAYER}";
    
    private const string PermissionPath = Paths.Realm + "/permission";
    private const string PlayerPermissionPath = PermissionPath + "/player/{PLAYER}";
    private const string DefaultPermissionPath = PermissionPath + "/default";
    
    private const string InviteCodePath = Paths.Realm + "/codes/invite";

    private readonly RestClient _restClient;

    public BedrockRealmService(IOptions<WebApiOptions> options, ClaimsPrincipal user)
    {
        var xstsToken = user.FindFirst(AuthClaims.BedrockXstsToken)!.Value;
        var userHash = user.FindFirst(AuthClaims.XboxLiveUserHash)!.Value;

        _restClient = new RestClient(new Uri(options.Value.Endpoint, BedrockPath),
            opts => opts.Authenticator = new XboxLiveAuthenticator(xstsToken, userHash));
        _restClient.AcceptedContentTypes = new[] { "*/*" };
    }

    public Task<BedrockRealm[]> GetRealmsAsync(CancellationToken cancellationToken = default) =>
        _restClient.GetJsonAsync<BedrockRealm[]>(Paths.Realms, cancellationToken)!;

    public Task<BedrockPlayer[]> GetOnlinePlayersAsync(long realmId, CancellationToken cancellationToken = default) =>
        _restClient.GetJsonAsync<BedrockPlayer[]>(Paths.Activity.Replace("{REALM}", realmId.ToString()),
            cancellationToken)!;

    public Task<Backup[]> GetBackupsAsync(long realmId, CancellationToken cancellationToken = default) =>
        _restClient.GetJsonAsync<Backup[]>(Paths.Backups.Replace("{REALM}", realmId.ToString()), cancellationToken)!;

    public async Task<BackupFile> DownloadBackupAsync(long realmId, int slotId, string? backupId = null,
        CancellationToken cancellationToken = default)
    {
        var request = new RestRequest(Paths.BackupDownload
            .Replace("{REALM}", realmId.ToString())
            .Replace("{SLOT}", slotId.ToString())
            .Replace("{BACKUP}", backupId ?? ""));

        var response = await _restClient.GetAsync(request, cancellationToken);

        var disposition = new ContentDisposition((string)response.ContentHeaders!
            .First(h => h.Name == KnownHeaders.ContentDisposition).Value!);

        return new BackupFile
        {
            Name = Path.GetFileNameWithoutExtension(disposition.FileName!),
            FileName = disposition.FileName!,
            Content = new MemoryStream(response.RawBytes!),
            ContentType = response.ContentType!
        };
    }

    public Task UploadBackupAsync(long realmId, int slotId, BackupFile backup,
        CancellationToken cancellationToken = default)
    {
        var request = new RestRequest(Paths.BackupUpload
                .Replace("{REALM}", realmId.ToString())
                .Replace("{SLOT}", slotId.ToString())
                .Replace("{BACKUP}", ""), Method.Post)
            .AddFile(backup.Name, () => backup.Content, backup.FileName, backup.ContentType);

        return _restClient.PostAsync(request, cancellationToken);
    }

    public Task RestoreBackupAsync(long realmId, string backupId, CancellationToken cancellationToken = default)
    {
        var request = new RestRequest(Paths.BackupUpload
                .Replace("{REALM}", realmId.ToString())
                .Replace("{SLOT}", "1")
                .Replace("{BACKUP}", backupId), Method.Post);

        return _restClient.PostAsync(request, cancellationToken);
    }

    public Task<BedrockRealm> GetRealmAsync(long realmId, CancellationToken cancellationToken = default) =>
        _restClient.GetJsonAsync<BedrockRealm>(Paths.Realm.Replace("{REALM}", realmId.ToString()), cancellationToken)!;

    public Task<string[]> GetBlockedPlayersAsync(long realmId, CancellationToken cancellationToken = default) =>
        _restClient.GetJsonAsync<string[]>(BlocklistPath.Replace("{REALM}", realmId.ToString()), cancellationToken)!;

    public Task BlockPlayerAsync(long realmId, string xuid, CancellationToken cancellationToken = default)
    {
        var request = new RestRequest(BlockPath
            .Replace("{REALM}", realmId.ToString())
            .Replace("{PLAYER}", xuid), Method.Post);

        return _restClient.PostAsync(request, cancellationToken);
    }
    
    public Task UnblockPlayerAsync(long realmId, string xuid, CancellationToken cancellationToken = default)
    {
        var request = new RestRequest(BlockPath
            .Replace("{REALM}", realmId.ToString())
            .Replace("{PLAYER}", xuid), Method.Delete);

        return _restClient.DeleteAsync(request, cancellationToken);
    }

    public Task ChangePlayerPermissionAsync(long realmId, string xuid, string permission,
        CancellationToken cancellationToken = default) =>
        _restClient.PutJsonAsync(PlayerPermissionPath
            .Replace("{REALM}", realmId.ToString())
            .Replace("{PLAYER}", xuid), $"\"{permission}\"", cancellationToken);

    public Task ChangeDefaultPermissionAsync(long realmId, string permission,
        CancellationToken cancellationToken = default) =>
        _restClient.PutJsonAsync(DefaultPermissionPath.Replace("{REALM}", realmId.ToString()), $"\"{permission}\"",
            cancellationToken);

    public Task<string> GetInviteCodeAsync(long realmId, CancellationToken cancellationToken = default) =>
        _restClient.GetJsonAsync<string>(InviteCodePath.Replace("{REALM}", realmId.ToString()), cancellationToken)!;

    public Task<string> GenerateInviteCodeAsync(long realmId, CancellationToken cancellationToken = default)
    {
        var request = new RestRequest(InviteCodePath.Replace("{REALM}", realmId.ToString()), Method.Post);

        return _restClient.PostAsync<string>(request, cancellationToken)!;
    }

    public Task CloseRealmAsync(long realmId, CancellationToken cancellationToken = default)
    {
        var request = new RestRequest(Paths.Close.Replace("{REALM}", realmId.ToString()), Method.Post);

        return _restClient.PostAsync(request, cancellationToken);
    }
    
    public Task OpenRealmAsync(long realmId, CancellationToken cancellationToken = default)
    {
        var request = new RestRequest(Paths.Open.Replace("{REALM}", realmId.ToString()), Method.Post);

        return _restClient.PostAsync(request, cancellationToken);
    }

    public Task ResetRealmAsync(long realmId, CancellationToken cancellationToken = default)
    {
        var request = new RestRequest(Paths.Reset.Replace("{REALM}", realmId.ToString()), Method.Post);

        return _restClient.PostAsync(request, cancellationToken);
    }

    public Task InvitePlayerAsync(long realmId, string player, CancellationToken cancellationToken = default)
    {
        var request = new RestRequest(Paths.Invite
            .Replace("{REALM}", realmId.ToString())
            .Replace("{PLAYER}", player), Method.Post);

        return _restClient.PostAsync(request, cancellationToken);
    }

    public Task UninvitePlayerAsync(long realmId, string player, CancellationToken cancellationToken = default)
    {
        var request = new RestRequest(Paths.Invite
            .Replace("{REALM}", realmId.ToString())
            .Replace("{PLAYER}", player), Method.Delete);

        return _restClient.DeleteAsync(request, cancellationToken);
    }

    public Task UpdateDescriptionAsync(long realmId, string name, string description,
        CancellationToken cancellationToken = default) =>
        _restClient.PutJsonAsync(Paths.Description.Replace("{REALM}", realmId.ToString()), new { name, description },
            cancellationToken);

    public Task UpdateBulkAsync(long realmId, int slotId, string name, string description,
        BedrockSlotOptionsDto options, CancellationToken cancellationToken = default)
    {
        var restRequest = new RestRequest(Paths.Slot
                .Replace("{REALM}", realmId.ToString())
                .Replace("{SLOT}", slotId.ToString()), Method.Patch)
            .AddBody(new { description = new { name, description }, options });

        return _restClient.PatchAsync(restRequest, cancellationToken);
    }

    public Task SwitchSlotAsync(long realmId, int slotId, CancellationToken cancellationToken = default)
    {
        var request = new RestRequest(Paths.Slot
            .Replace("{REALM}", realmId.ToString())
            .Replace("{SLOT}", slotId.ToString()), Method.Put);

        return _restClient.PutAsync(request, cancellationToken);
    }

    public void Dispose() =>
        _restClient.Dispose();
}

public class JavaRealmService : IJavaRealmService
{
    private const string JavaPath = "/java";

    private const string SlotOptionsPath = Paths.Slot + "/options";

    private const string OpPath = Paths.Realm + "/ops/{PLAYER}";

    private readonly RestClient _restClient;

    public JavaRealmService(IOptions<WebApiOptions> options, ClaimsPrincipal user)
    {
        var accessToken = user.FindFirstValue(AuthClaims.JavaAccessToken)!;
        var username = user.FindFirstValue(AuthClaims.JavaUser)!;
        var uuid = user.FindFirstValue(AuthClaims.JavaUuid)!;

        _restClient = new RestClient(new Uri(options.Value.Endpoint, JavaPath),
            opts => opts.Authenticator =
                new JavaAuthenticator(accessToken, uuid, username, MinecraftConstants.JavaVersion));
        _restClient.AcceptedContentTypes = new[] { "*/*" };
    }
    
    public Task<JavaRealm[]> GetRealmsAsync(CancellationToken cancellationToken = default) =>
        _restClient.GetJsonAsync<JavaRealm[]>(Paths.Realms, cancellationToken)!;

    public Task<JavaRealm> GetRealmAsync(long realmId, CancellationToken cancellationToken = default) =>
        _restClient.GetJsonAsync<JavaRealm>(Paths.Realm.Replace("{REALM}", realmId.ToString()), cancellationToken)!;

    public Task<string[]> GetOnlinePlayersAsync(long realmId, CancellationToken cancellationToken = default) =>
        _restClient.GetJsonAsync<string[]>(Paths.Activity.Replace("{REALM}", realmId.ToString()), cancellationToken)!;

    public Task<Backup[]> GetBackupsAsync(long realmId, CancellationToken cancellationToken = default) =>
        _restClient.GetJsonAsync<Backup[]>(Paths.Backups.Replace("{REALM}", realmId.ToString()), cancellationToken)!;

    public async Task<BackupFile> DownloadBackupAsync(long realmId, int slotId, string? backupId = null,
        CancellationToken cancellationToken = default)
    {
        var request = new RestRequest(Paths.BackupDownload
            .Replace("{REALM}", realmId.ToString())
            .Replace("{SLOT}", slotId.ToString())
            .Replace("{BACKUP}", backupId ?? ""));

        var response = await _restClient.GetAsync(request, cancellationToken);

        var disposition = new ContentDisposition((string)response.ContentHeaders!
            .First(h => h.Name == KnownHeaders.ContentDisposition).Value!);

        return new BackupFile
        {
            Name = Path.GetFileNameWithoutExtension(disposition.FileName!),
            FileName = disposition.FileName!,
            Content = new MemoryStream(response.RawBytes!),
            ContentType = response.ContentType!
        };
    }

    public Task UploadBackupAsync(long realmId, int slotId, BackupFile backup,
        CancellationToken cancellationToken = default)
    {
        var request = new RestRequest(Paths.BackupUpload
                .Replace("{REALM}", realmId.ToString())
                .Replace("{SLOT}", slotId.ToString())
                .Replace("{BACKUP}", ""), Method.Post)
            .AddFile(backup.Name, () => backup.Content, backup.FileName, backup.ContentType);

        return _restClient.PostAsync(request, cancellationToken);
    }

    public Task RestoreBackupAsync(long realmId, string backupId, CancellationToken cancellationToken = default)
    {
        var request = new RestRequest(Paths.BackupUpload
            .Replace("{REALM}", realmId.ToString())
            .Replace("{SLOT}", "1")
            .Replace("{BACKUP}", backupId), Method.Post);

        return _restClient.PostAsync(request, cancellationToken);
    }

    public Task CloseRealmAsync(long realmId, CancellationToken cancellationToken = default)
    {
        var request = new RestRequest(Paths.Close.Replace("{REALM}", realmId.ToString()), Method.Post);

        return _restClient.PostAsync(request, cancellationToken);
    }
    
    public Task OpenRealmAsync(long realmId, CancellationToken cancellationToken = default)
    {
        var request = new RestRequest(Paths.Open.Replace("{REALM}", realmId.ToString()), Method.Post);

        return _restClient.PostAsync(request, cancellationToken);
    }

    public Task ResetRealmAsync(long realmId, CancellationToken cancellationToken = default)
    {
        var request = new RestRequest(Paths.Reset.Replace("{REALM}", realmId.ToString()), Method.Post);

        return _restClient.PostAsync(request, cancellationToken);
    }

    public Task InvitePlayerAsync(long realmId, string player, CancellationToken cancellationToken = default)
    {
        var request = new RestRequest(Paths.Invite
            .Replace("{REALM}", realmId.ToString())
            .Replace("{PLAYER}", player), Method.Post);

        return _restClient.PostAsync(request, cancellationToken);
    }

    public Task UninvitePlayerAsync(long realmId, string player, CancellationToken cancellationToken = default)
    {
        var request = new RestRequest(Paths.Invite
            .Replace("{REALM}", realmId.ToString())
            .Replace("{PLAYER}", player), Method.Delete);

        return _restClient.DeleteAsync(request, cancellationToken);
    }

    public Task UpdateDescriptionAsync(long realmId, string name, string description,
        CancellationToken cancellationToken = default) =>
        _restClient.PutJsonAsync(Paths.Description.Replace("{REALM}", realmId.ToString()), new { name, description },
            cancellationToken);

    public Task UpdateBulkAsync(long realmId, int slotId, string name, string description,
        SlotOptionsDto options, CancellationToken cancellationToken = default)
    {
        var restRequest = new RestRequest(Paths.Slot
                .Replace("{REALM}", realmId.ToString())
                .Replace("{SLOT}", slotId.ToString()), Method.Patch)
            .AddBody(new { description = new { name, description }, options });

        return _restClient.PatchAsync(restRequest, cancellationToken);
    }

    public Task SwitchSlotAsync(long realmId, int slotId, CancellationToken cancellationToken = default)
    {
        var request = new RestRequest(Paths.Slot
            .Replace("{REALM}", realmId.ToString())
            .Replace("{SLOT}", slotId.ToString()), Method.Put);

        return _restClient.PutAsync(request, cancellationToken);
    }

    public Task UpdateSlotOptionsAsync(long realmId, int slotId, SlotOptionsDto options,
        CancellationToken cancellationToken = default) =>
        _restClient.PutJsonAsync(SlotOptionsPath
            .Replace("{REALM}", realmId.ToString())
            .Replace("{SLOT}", slotId.ToString()), options, cancellationToken);

    public Task OpAsync(long realmId, string player, CancellationToken cancellationToken = default)
    {
        var request = new RestRequest(OpPath
            .Replace("{REALM}", realmId.ToString())
            .Replace("{PLAYER}", player), Method.Post);

        return _restClient.PostAsync(request, cancellationToken);
    }

    public Task DeopAsync(long realmId, string player, CancellationToken cancellationToken = default)
    {
        var request = new RestRequest(OpPath
            .Replace("{REALM}", realmId.ToString())
            .Replace("{PLAYER}", player), Method.Delete);

        return _restClient.DeleteAsync(request, cancellationToken);
    }
}