using System.Net;
using System.Net.Mime;
using System.Security.Claims;
using AutoMapper;

using RestSharp;
using RestSharp.Authenticators;

using RealmHelper.Application.Models.Minecraft;
using RealmHelper.Application.Models.Minecraft.Requests;
using RealmHelper.Application.Models.Minecraft.Responses;
using RealmHelper.Application.Services;
using RealmHelper.Domain.Models.Minecraft;
using RealmHelper.Domain.Models.Minecraft.Bedrock;

using Microsoft.Extensions.Caching.Memory;
using RealmHelper.Application;
using RealmHelper.Application.Api;
using RealmHelper.Application.Authentication;
using RealmHelper.Domain.Models.Minecraft.Java;

namespace RealmHelper.Infrastructure.Services;

public class BedrockRealmService : IBedrockRealmService
{
   private readonly IBedrockRealmApi _realmApi;

    private readonly IMemoryCache _cache;
    
    private readonly IMapper _mapper;

    public BedrockRealmService(IBedrockRealmApi realmApi, IMemoryCache cache, IMapper mapper) =>
        (_realmApi, _cache, _mapper) = (realmApi, cache, mapper);

    public async Task<BedrockRealm[]> GetRealmsAsync(CancellationToken cancellationToken = default) =>
        _mapper.Map<BedrockRealm[]>(await _realmApi.GetRealmsAsync(cancellationToken));

    public async Task<BedrockRealm> GetRealmAsync(long realmId, CancellationToken cancellationToken = default) =>
        _mapper.Map<BedrockRealm>(await _realmApi.GetRealmAsync(realmId, cancellationToken));

    public async Task<BedrockPlayer[]> GetOnlinePlayersAsync(long realmId, CancellationToken cancellationToken = default)
    {
        var response = await _realmApi.GetPlayerActivitiesAsync(cancellationToken);

        var players = (from r in response.Servers where r.RealmId == realmId select r.Players).FirstOrDefault();

        return players is null ? Array.Empty<BedrockPlayer>() : _mapper.Map<BedrockPlayer[]>(players);
    }

    public async Task<Backup[]> GetBackupsAsync(long realmId, CancellationToken cancellationToken = default) =>
        _mapper.Map<Backup[]>(await _realmApi.GetBackupsAsync(realmId, cancellationToken));

    public async Task<BackupFile> DownloadBackupAsync(long realmId, int slotId, string? backupId = null,
        CancellationToken cancellationToken = default)
    {
        var archiveData = await _realmApi.RequestDownloadInfo(realmId, slotId, backupId, cancellationToken);

        var client = new RestClient(opts => opts.Authenticator = new JwtAuthenticator(archiveData.Token!))
            .AddDefaultHeader(KnownHeaders.Accept, "*/*");

        var response = await client.GetAsync(new RestRequest(archiveData.Url), cancellationToken);

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

    public async Task UploadBackupAsync(long realmId, int slotId, BackupFile backup,
        CancellationToken cancellationToken = default)
    {
        if (!_cache.TryGetValue(realmId, out ArchiveResponse? archiveData))
        {
            archiveData = await _realmApi.RequestUploadInfo(realmId, slotId, cancellationToken);

            _cache.Set(realmId, archiveData, TimeSpan.FromMinutes(15));
        }

        var client = new RestClient(opts => opts.Authenticator = new JwtAuthenticator(archiveData!.Token!))
            .AddDefaultHeader(KnownHeaders.Accept, "*/*");

        var request = new RestRequest(archiveData!.Url, Method.Post)
            .AddFile(backup.Name, () => backup.Content, backup.FileName, backup.ContentType);
        var response = await client.PostAsync(request, cancellationToken);

        if (response.Content!.Contains("FAILED"))
            throw new ArgumentException("Invalid archive data", nameof(backup));
    }

    public async Task RestoreBackupAsync(long realmId, string backupId, CancellationToken cancellationToken = default) =>
        await _realmApi.RestoreBackupAsync(realmId, backupId, cancellationToken);

    public async Task CloseRealmAsync(long realmId, CancellationToken cancellationToken = default) =>
        await _realmApi.CloseRealmAsync(realmId, cancellationToken);

    public async Task OpenRealmAsync(long realmId, CancellationToken cancellationToken = default) =>
        await _realmApi.OpenRealmAsync(realmId, cancellationToken);

    public async Task ResetRealmAsync(long realmId, CancellationToken cancellationToken = default) =>
        await _realmApi.ResetRealmAsync(realmId, cancellationToken);

    public async Task InvitePlayerAsync(long realmId, string xuid, CancellationToken cancellationToken = default) =>
        await _realmApi.InvitePlayerAsync(realmId, xuid, cancellationToken);

    public async Task UninvitePlayerAsync(long realmId, string xuid, CancellationToken cancellationToken = default) =>
        await _realmApi.UninvitePlayerAsync(realmId, xuid, cancellationToken);

    public async Task UpdateDescriptionAsync(long realmId, string name, string description,
        CancellationToken cancellationToken = default) =>
        await _realmApi.UpdateDescriptionAsync(realmId,
            new DescriptionUpdateRequest { Name = name, Description = description }, cancellationToken);

    public async Task UpdateBulkAsync(long realmId, int slotId, string name, string description,
        BedrockSlotOptionsDto options,
        CancellationToken cancellationToken = default) =>
        await _realmApi.UpdateSlotAsync(realmId, slotId,
            new SlotUpdateRequest<BedrockSlotOptionsDto>
            {
                Description = new DescriptionUpdateRequest { Name = name, Description = description }, Options = options
            }, cancellationToken);

    public async Task SwitchSlotAsync(long realmId, int slotId, CancellationToken cancellationToken = default) =>
        await _realmApi.ActivateSlotAsync(realmId, slotId, cancellationToken);

    public async Task<string[]> GetBlockedPlayersAsync(long realmId, CancellationToken cancellationToken = default) =>
        await _realmApi.GetBlockedPlayersAsync(realmId, cancellationToken);

    public async Task BlockPlayerAsync(long realmId, string xuid, CancellationToken cancellationToken = default) =>
        await _realmApi.BlockPlayerAsync(realmId, xuid, cancellationToken);

    public async Task UnblockPlayerAsync(long realmId, string xuid, CancellationToken cancellationToken = default) =>
        await _realmApi.UnblockPlayerAsync(realmId, xuid, cancellationToken);

    public async Task ChangePlayerPermissionAsync(long realmId, string xuid, string permission,
        CancellationToken cancellationToken = default) =>
        await _realmApi.ChangePlayerPermissionAsync(realmId,
            new PlayerPermissionChangeRequest { Xuid = xuid, Permission = permission }, cancellationToken);

    public async Task ChangeDefaultPermissionAsync(long realmId, string permission,
        CancellationToken cancellationToken = default) =>
        await _realmApi.ChangeDefaultPermissionAsync(realmId,
            new PermissionChangeRequest { Permission = permission }, cancellationToken);

    public async Task<string> GetInviteCodeAsync(long realmId, CancellationToken cancellationToken = default)
    {
        var response = await _realmApi.GetInviteCodesAsync(realmId, cancellationToken);

        return response[0].LinkId;
    }

    public async Task<string> GenerateInviteCodeAsync(long realmId, CancellationToken cancellationToken = default)
    {
        var response = await _realmApi.GenerateInviteCodeAsync(new InviteLinkRequest
            { Type = "INFINITE", WorldId = realmId }, cancellationToken);

        return response.LinkId;
    }
}

public class JavaRealmService : IJavaRealmService
{
    private readonly IJavaRealmApi _realmApi;

    private readonly IMapper _mapper;

    private readonly IMemoryCache _cache;

    private readonly ClaimsPrincipal _user;

    public JavaRealmService(
        IJavaRealmApi realmApi, 
        IMapper mapper, 
        IMemoryCache cache, 
        ClaimsPrincipal user
    )
    {
        _realmApi = realmApi;
        _mapper = mapper;
        _cache = cache;
        _user = user;
    }

    public async Task<JavaRealm[]> GetRealmsAsync(CancellationToken cancellationToken = default) =>
        _mapper.Map<JavaRealm[]>(await _realmApi.GetRealmsAsync(cancellationToken));

    public async Task<JavaRealm> GetRealmAsync(long realmId, CancellationToken cancellationToken = default) =>
        _mapper.Map<JavaRealm>(await _realmApi.GetRealmAsync(realmId, cancellationToken));

    public async Task<string[]> GetOnlinePlayersAsync(long realmId, CancellationToken cancellationToken = default)
    {
        var activities = await _realmApi.GetPlayerActivitiesAsync(cancellationToken);

        var players = (from server in activities.Servers
            where server.RealmId == realmId
            select server.Players).FirstOrDefault();

        return players ?? Array.Empty<string>();
    }

    public async Task<Backup[]> GetBackupsAsync(long realmId, CancellationToken cancellationToken = default) =>
        _mapper.Map<Backup[]>(await _realmApi.GetBackupsAsync(realmId, cancellationToken));

    public async Task<BackupFile> DownloadBackupAsync(long realmId, int slotId, string? backupId = null,
        CancellationToken cancellationToken = default)
    {
        var archiveData = await _realmApi.RequestDownloadInfo(realmId, slotId, backupId, cancellationToken);

        var client = new RestClient();
        client.AcceptedContentTypes = new[] { "*/*" };

        var response = await client.GetAsync(new RestRequest(archiveData.Url), cancellationToken);

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

    public async Task UploadBackupAsync(long realmId, int slotId, BackupFile backup,
        CancellationToken cancellationToken = default)
    {
        if (!_cache.TryGetValue(realmId, out ArchiveResponse? archiveData))
        {
            archiveData = await _realmApi.RequestUploadInfo(realmId, slotId, cancellationToken);

            _cache.Set(realmId, archiveData, TimeSpan.FromMinutes(15));
        }

        var uuid = _user.FindFirst(AuthClaims.JavaUuid)!.Value;
        var user = _user.FindFirst(AuthClaims.JavaUser)!.Value;
        var token = _user.FindFirst(AuthClaims.JavaAccessToken)!.Value;

        var client = new RestClient(archiveData!.Url, opts =>
        {
            opts.CookieContainer = new CookieContainer();
            opts.CookieContainer.Add(new Cookie("sid", $"token:{token}:{uuid}"));
            opts.CookieContainer.Add(new Cookie("user", user));
            opts.CookieContainer.Add(new Cookie("version", MinecraftConstants.Version));
            opts.CookieContainer.Add(new Cookie("token", archiveData.Token));
        });
        client.AcceptedContentTypes = new[] { "*/*" };

        var request = new RestRequest($"/upload/{realmId}/${slotId}", Method.Post)
            .AddFile(backup.Name, () => backup.Content, backup.FileName, backup.ContentType);
        var response = await client.PostAsync(request, cancellationToken);

        if (response.Content!.Contains("FAILED"))
            throw new ArgumentException("Invalid Archive Data", nameof(backup));
    }

    public Task RestoreBackupAsync(long realmId, string backupId, CancellationToken cancellationToken = default) =>
        _realmApi.RestoreBackupAsync(realmId, backupId, cancellationToken);

    public Task CloseRealmAsync(long realmId, CancellationToken cancellationToken = default) =>
        _realmApi.CloseRealmAsync(realmId, cancellationToken);

    public Task OpenRealmAsync(long realmId, CancellationToken cancellationToken = default) =>
        _realmApi.OpenRealmAsync(realmId, cancellationToken);

    public Task ResetRealmAsync(long realmId, CancellationToken cancellationToken = default) =>
        _realmApi.ResetRealmAsync(realmId, cancellationToken);

    public Task InvitePlayerAsync(long realmId, string player, CancellationToken cancellationToken = default) =>
        _realmApi.InvitePlayerAsync(realmId, player, cancellationToken);

    public Task UninvitePlayerAsync(long realmId, string player, CancellationToken cancellationToken = default) =>
        _realmApi.UninvitePlayerAsync(realmId, player, cancellationToken);

    public Task UpdateDescriptionAsync(long realmId, string name, string description,
        CancellationToken cancellationToken = default) =>
        _realmApi.UpdateDescriptionAsync(realmId,
            new DescriptionUpdateRequest { Name = name, Description = description }, cancellationToken);

    public Task UpdateBulkAsync(long realmId, int slotId, string name, string description, SlotOptionsDto options,
        CancellationToken cancellationToken = default) =>
        _realmApi.UpdateSlotAsync(realmId, slotId,
            new SlotUpdateRequest<SlotOptionsDto>
            {
                Description = new DescriptionUpdateRequest { Name = name, Description = description }, Options = options
            }, cancellationToken);

    public Task UpdateSlotOptionsAsync(long realmId, int slotId, SlotOptionsDto options,
        CancellationToken cancellationToken = default) =>
        _realmApi.UpdateSlotAsync(realmId, slotId, options, cancellationToken);

    public Task SwitchSlotAsync(long realmId, int slotId, CancellationToken cancellationToken = default) =>
        _realmApi.ActivateSlotAsync(realmId, slotId, cancellationToken);

    public Task OpAsync(long realmId, string player, CancellationToken cancellationToken = default) =>
        _realmApi.OpAsync(realmId, player, cancellationToken);
    
    public Task DeopAsync(long realmId, string player, CancellationToken cancellationToken = default) =>
        _realmApi.DeopAsync(realmId, player, cancellationToken);
}