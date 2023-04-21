using System.Net.Mime;

using AutoMapper;

using RestSharp;
using RestSharp.Authenticators;

using Microsoft.Extensions.Caching.Memory;

using RealmHelper.Realm.Bedrock.Abstractions.Models;
using RealmHelper.Realm.Bedrock.Abstractions.Services;
using RealmHelper.Realm.Bedrock.Application.Models.Requests;
using RealmHelper.Realm.Bedrock.Application.Repositories;
using RealmHelper.Realm.Common.Abstractions.Models;
using RealmHelper.Realm.Common.Application.Models.Requests;

namespace RealmHelper.Realm.Bedrock.Infrastructure.Services;

public class BedrockRealmService : IBedrockRealmService
{
    private readonly IBedrockRealmRepository _realmRepository;

    private readonly IMemoryCache _cache;
    
    private readonly IMapper _mapper;

    public BedrockRealmService(IBedrockRealmRepository realmRepository, IMemoryCache cache, IMapper mapper) =>
        (_realmRepository, _cache, _mapper) = (realmRepository, cache, mapper);

    public async Task<BedrockRealm[]> GetRealmsAsync(CancellationToken cancellationToken = default) =>
        _mapper.Map<BedrockRealm[]>(await _realmRepository.GetRealmsAsync(cancellationToken));

    public async Task<BedrockRealm> GetRealmAsync(long realmId, CancellationToken cancellationToken = default) =>
        _mapper.Map<BedrockRealm>(await _realmRepository.GetRealmAsync(realmId, cancellationToken));

    public async Task<BedrockPlayer[]> GetOnlinePlayersAsync(long realmId, CancellationToken cancellationToken = default)
    {
        var response = await _realmRepository.GetPlayerActivitiesAsync(cancellationToken);

        var players = (from r in response.Servers where r.RealmId == realmId select r.Players).FirstOrDefault();

        return players is null ? Array.Empty<BedrockPlayer>() : _mapper.Map<BedrockPlayer[]>(players);
    }

    public async Task<Backup[]> GetBackupsAsync(long realmId, CancellationToken cancellationToken = default) =>
        _mapper.Map<Backup[]>(await _realmRepository.GetBackupsAsync(realmId, cancellationToken));

    public async Task<BackupFile> DownloadBackupAsync(long realmId, int slotId, string? backupId = null,
        CancellationToken cancellationToken = default)
    {
        var archiveData = await _realmRepository.RequestDownloadInfo(realmId, slotId, backupId, cancellationToken);

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
        if (!_cache.TryGetValue(realmId, out ArchiveInfo? archiveData))
        {
            archiveData = await _realmRepository.RequestUploadInfo(realmId, slotId, cancellationToken);

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
        await _realmRepository.RestoreBackupAsync(realmId, backupId, cancellationToken);

    public async Task CloseRealmAsync(long realmId, CancellationToken cancellationToken = default) =>
        await _realmRepository.CloseRealmAsync(realmId, cancellationToken);

    public async Task OpenRealmAsync(long realmId, CancellationToken cancellationToken = default) =>
        await _realmRepository.OpenRealmAsync(realmId, cancellationToken);

    public async Task ResetRealmAsync(long realmId, CancellationToken cancellationToken = default) =>
        await _realmRepository.ResetRealmAsync(realmId, cancellationToken);

    public async Task InvitePlayerAsync(long realmId, string xuid, CancellationToken cancellationToken = default) =>
        await _realmRepository.InvitePlayerAsync(realmId, xuid, cancellationToken);

    public async Task UninvitePlayerAsync(long realmId, string xuid, CancellationToken cancellationToken = default) =>
        await _realmRepository.UninvitePlayerAsync(realmId, xuid, cancellationToken);

    public async Task UpdateDescriptionAsync(long realmId, string name, string description,
        CancellationToken cancellationToken = default) =>
        await _realmRepository.UpdateDescriptionAsync(realmId,
            new DescriptionUpdateRequest { Name = name, Description = description }, cancellationToken);

    public async Task UpdateBulkAsync(long realmId, int slotId, string name, string description,
        BedrockSlotOptionsDto options,
        CancellationToken cancellationToken = default) =>
        await _realmRepository.UpdateSlotAsync(realmId, slotId,
            new SlotUpdateRequest<BedrockSlotOptionsDto>
            {
                Description = new DescriptionUpdateRequest { Name = name, Description = description }, Options = options
            }, cancellationToken);

    public async Task SwitchSlotAsync(long realmId, int slotId, CancellationToken cancellationToken = default) =>
        await _realmRepository.ActivateSlotAsync(realmId, slotId, cancellationToken);

    public async Task<string[]> GetBlockedPlayersAsync(long realmId, CancellationToken cancellationToken = default) =>
        await _realmRepository.GetBlockedPlayersAsync(realmId, cancellationToken);

    public async Task BlockPlayerAsync(long realmId, string xuid, CancellationToken cancellationToken = default) =>
        await _realmRepository.BlockPlayerAsync(realmId, xuid, cancellationToken);

    public async Task UnblockPlayerAsync(long realmId, string xuid, CancellationToken cancellationToken = default) =>
        await _realmRepository.UnblockPlayerAsync(realmId, xuid, cancellationToken);

    public async Task ChangePlayerPermissionAsync(long realmId, string xuid, string permission,
        CancellationToken cancellationToken = default) =>
        await _realmRepository.ChangePlayerPermissionAsync(realmId,
            new PlayerPermissionChangeRequest { Xuid = xuid, Permission = permission }, cancellationToken);

    public async Task ChangeDefaultPermissionAsync(long realmId, string permission,
        CancellationToken cancellationToken = default) =>
        await _realmRepository.ChangeDefaultPermissionAsync(realmId,
            new PermissionChangeRequest { Permission = permission }, cancellationToken);

    public async Task<string> GetInviteCodeAsync(long realmId, CancellationToken cancellationToken = default)
    {
        var response = await _realmRepository.GetInviteCodesAsync(realmId, cancellationToken);

        return response[0].LinkId;
    }

    public async Task<string> GenerateInviteCodeAsync(long realmId, CancellationToken cancellationToken = default)
    {
        var response = await _realmRepository.GenerateInviteCodeAsync(new InviteLinkRequest
            { Type = "INFINITE", WorldId = realmId }, cancellationToken);

        return response.LinkId;
    }
}