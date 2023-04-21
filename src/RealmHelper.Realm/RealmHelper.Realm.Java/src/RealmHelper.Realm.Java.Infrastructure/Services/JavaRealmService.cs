using System.Net;
using System.Net.Mime;
using System.Security.Claims;

using AutoMapper;
using RestSharp;

using Microsoft.Extensions.Caching.Memory;

using RealmHelper.Realm.Common.Abstractions.Models;
using RealmHelper.Realm.Common.Application.Models.Requests;
using RealmHelper.Realm.Java.Abstractions.Models;
using RealmHelper.Realm.Java.Abstractions.Services;
using RealmHelper.Realm.Java.Application.Repositories;
using RealmHelper.Realm.Java.Infrastructure.Authentication;

namespace RealmHelper.Realm.Java.Infrastructure.Services;

public class JavaRealmService : IJavaRealmService
{
    private readonly IJavaRealmRepository _realmRepository;

    private readonly IMapper _mapper;

    private readonly IMemoryCache _cache;

    private readonly ClaimsPrincipal _user;

    public JavaRealmService(
        IJavaRealmRepository realmRepository, 
        IMapper mapper, 
        IMemoryCache cache, 
        ClaimsPrincipal user
    )
    {
        _realmRepository = realmRepository;

        _mapper = mapper;

        _cache = cache;

        _user = user;
    }

    public async Task<JavaRealm[]> GetRealmsAsync(CancellationToken cancellationToken = default) =>
        _mapper.Map<JavaRealm[]>(await _realmRepository.GetRealmsAsync(cancellationToken));

    public async Task<JavaRealm> GetRealmAsync(long realmId, CancellationToken cancellationToken = default) =>
        _mapper.Map<JavaRealm>(await _realmRepository.GetRealmAsync(realmId, cancellationToken));

    public async Task<string[]> GetOnlinePlayersAsync(long realmId, CancellationToken cancellationToken = default)
    {
        var activities = await _realmRepository.GetPlayerActivitiesAsync(cancellationToken);

        var players = (from server in activities.Servers
            where server.RealmId == realmId
            select server.Players).FirstOrDefault();

        return players ?? Array.Empty<string>();
    }

    public async Task<Backup[]> GetBackupsAsync(long realmId, CancellationToken cancellationToken = default) =>
        _mapper.Map<Backup[]>(await _realmRepository.GetBackupsAsync(realmId, cancellationToken));

    public async Task<BackupFile> DownloadBackupAsync(long realmId, int slotId, string? backupId = null,
        CancellationToken cancellationToken = default)
    {
        var archiveData = await _realmRepository.RequestDownloadInfo(realmId, slotId, backupId, cancellationToken);

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
        if (!_cache.TryGetValue(realmId, out ArchiveInfo? archiveData))
        {
            archiveData = await _realmRepository.RequestUploadInfo(realmId, slotId, cancellationToken);

            _cache.Set(realmId, archiveData, TimeSpan.FromMinutes(15));
        }

        var client = new RestClient(archiveData!.Url, opts =>
        {
            opts.CookieContainer = new CookieContainer();
            opts.CookieContainer.Add(new Cookie("sid", _user.FindFirst(AuthClaims.Sid)!.Value));
            opts.CookieContainer.Add(new Cookie("user", _user.FindFirst(AuthClaims.User)!.Value));
            opts.CookieContainer.Add(new Cookie("version", _user.FindFirst(AuthClaims.Version)!.Value));
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
        _realmRepository.RestoreBackupAsync(realmId, backupId, cancellationToken);

    public Task CloseRealmAsync(long realmId, CancellationToken cancellationToken = default) =>
        _realmRepository.CloseRealmAsync(realmId, cancellationToken);

    public Task OpenRealmAsync(long realmId, CancellationToken cancellationToken = default) =>
        _realmRepository.OpenRealmAsync(realmId, cancellationToken);

    public Task ResetRealmAsync(long realmId, CancellationToken cancellationToken = default) =>
        _realmRepository.ResetRealmAsync(realmId, cancellationToken);

    public Task InvitePlayerAsync(long realmId, string player, CancellationToken cancellationToken = default) =>
        _realmRepository.InvitePlayerAsync(realmId, player, cancellationToken);

    public Task UninvitePlayerAsync(long realmId, string player, CancellationToken cancellationToken = default) =>
        _realmRepository.UninvitePlayerAsync(realmId, player, cancellationToken);

    public Task UpdateDescriptionAsync(long realmId, string name, string description,
        CancellationToken cancellationToken = default) =>
        _realmRepository.UpdateDescriptionAsync(realmId,
            new DescriptionUpdateRequest { Name = name, Description = description }, cancellationToken);

    public Task UpdateBulkAsync(long realmId, int slotId, string name, string description, SlotOptionsDto options,
        CancellationToken cancellationToken = default) =>
        _realmRepository.UpdateSlotAsync(realmId, slotId,
            new SlotUpdateRequest<SlotOptionsDto>
            {
                Description = new DescriptionUpdateRequest { Name = name, Description = description }, Options = options
            }, cancellationToken);

    public Task UpdateSlotOptionsAsync(long realmId, int slotId, SlotOptionsDto options,
        CancellationToken cancellationToken = default) =>
        _realmRepository.UpdateSlotAsync(realmId, slotId, options, cancellationToken);

    public Task SwitchSlotAsync(long realmId, int slotId, CancellationToken cancellationToken = default) =>
        _realmRepository.ActivateSlotAsync(realmId, slotId, cancellationToken);

    public Task OpAsync(long realmId, string player, CancellationToken cancellationToken = default) =>
        _realmRepository.OpAsync(realmId, player, cancellationToken);
    
    public Task DeopAsync(long realmId, string player, CancellationToken cancellationToken = default) =>
        _realmRepository.DeopAsync(realmId, player, cancellationToken);
}