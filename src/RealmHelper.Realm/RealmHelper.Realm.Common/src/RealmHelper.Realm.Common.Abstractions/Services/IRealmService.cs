using RealmHelper.Realm.Common.Abstractions.Models;

namespace RealmHelper.Realm.Common.Abstractions.Services;

public interface IRealmService<TRealm, TActivityPlayer, in TSlotOptions>
    where TRealm : BaseRealm
    where TSlotOptions : SlotOptionsDto
{
    Task<TRealm[]> GetRealmsAsync(CancellationToken cancellationToken = default);
    
    Task<TRealm> GetRealmAsync(long realmId, CancellationToken cancellationToken = default);
    
    Task<TActivityPlayer[]> GetOnlinePlayersAsync(long realmId, CancellationToken cancellationToken = default);

    Task<Backup[]> GetBackupsAsync(long realmId, CancellationToken cancellationToken = default);

    Task<BackupFile> DownloadBackupAsync(long realmId, int slotId, string? backupId = default,
        CancellationToken cancellationToken = default);
    
    Task UploadBackupAsync(long realmId, int slotId, BackupFile backup, CancellationToken cancellationToken = default);

    Task RestoreBackupAsync(long realmId, string backupId, CancellationToken cancellationToken = default);

    Task CloseRealmAsync(long realmId, CancellationToken cancellationToken = default);
    
    Task OpenRealmAsync(long realmId, CancellationToken cancellationToken = default);
    
    Task ResetRealmAsync(long realmId, CancellationToken cancellationToken = default);

    Task InvitePlayerAsync(long realmId, string player, CancellationToken cancellationToken = default);
    
    Task UninvitePlayerAsync(long realmId, string player, CancellationToken cancellationToken = default);

    Task UpdateDescriptionAsync(long realmId, string name, string description,
        CancellationToken cancellationToken = default);

    Task UpdateBulkAsync(long realmId, int slotId, string name, string description, TSlotOptions slotOptions,
        CancellationToken cancellationToken = default);
    
    Task SwitchSlotAsync(long realmId, int slotId, CancellationToken cancellationToken = default);
}