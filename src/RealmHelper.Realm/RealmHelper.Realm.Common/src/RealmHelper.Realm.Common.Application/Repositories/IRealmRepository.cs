using RealmHelper.Realm.Common.Abstractions.Models;
using RealmHelper.Realm.Common.Application.Models.Requests;
using RealmHelper.Realm.Common.Application.Models.Responses;

namespace RealmHelper.Realm.Common.Application.Repositories;

public interface IRealmRepository<TRealm, TActivityPlayer, TActivityResponse, TSlotOptions>
    where TRealm : BaseRealmResponse
    where TActivityResponse : PlayerActivityResponse<TActivityPlayer>
    where TSlotOptions : SlotOptionsDto
{
    Task<RealmsResponse<TRealm>> GetRealmsAsync(CancellationToken cancellationToken = default);
    
    Task<TRealm> GetRealmAsync(long realmId, CancellationToken cancellationToken = default);
    
    Task CloseRealmAsync(long realmId, CancellationToken cancellationToken = default);
    
    Task OpenRealmAsync(long realmId, CancellationToken cancellationToken = default);

    Task ResetRealmAsync(long realmId, CancellationToken cancellationToken = default);

    Task<BackupsResponse> GetBackupsAsync(long realmId, CancellationToken cancellationToken = default);

    Task<ArchiveResponse> RequestDownloadInfo(long realmId, int slotId, string? backupId = default,
        CancellationToken cancellationToken = default);
    
    Task<ArchiveResponse> RequestUploadInfo(long realmId, int slotId, CancellationToken cancellationToken = default);

    Task RestoreBackupAsync(long realmId, string backupId, CancellationToken cancellationToken = default);

    Task InvitePlayerAsync(long realmId, string player, CancellationToken cancellationToken = default);
    
    Task UninvitePlayerAsync(long realmId, string player, CancellationToken cancellationToken = default);

    Task ActivateSlotAsync(long realmId, int slotId, CancellationToken cancellationToken = default);

    Task UpdateDescriptionAsync(long realmId, DescriptionUpdateRequest descriptionRequest,
        CancellationToken cancellationToken = default);

    Task UpdateSlotAsync(long realmId, int slotId, SlotUpdateRequest<TSlotOptions> request,
        CancellationToken cancellationToken = default);

    Task<PlayerActivitiesResponse<TActivityPlayer, TActivityResponse>> GetPlayerActivitiesAsync(
        CancellationToken cancellationToken = default);
}