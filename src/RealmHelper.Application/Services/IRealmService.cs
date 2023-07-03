using RealmHelper.Application.Models.Minecraft;
using RealmHelper.Domain.Models.Minecraft;
using RealmHelper.Domain.Models.Minecraft.Bedrock;
using RealmHelper.Domain.Models.Minecraft.Java;

namespace RealmHelper.Application.Services;

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

public interface IBedrockRealmService : IRealmService<BedrockRealm, BedrockPlayer, BedrockSlotOptionsDto>
{
    Task<string[]> GetBlockedPlayersAsync(long realmId, CancellationToken cancellationToken = default);

    Task BlockPlayerAsync(long realmId, string xuid, CancellationToken cancellationToken = default);

    Task UnblockPlayerAsync(long realmId, string xuid, CancellationToken cancellationToken = default);

    Task ChangePlayerPermissionAsync(long realmId, string xuid, string permission,
        CancellationToken cancellationToken = default);

    Task ChangeDefaultPermissionAsync(long realmId, string permission, CancellationToken cancellationToken = default);

    Task<string> GetInviteCodeAsync(long realmId, CancellationToken cancellationToken = default);

    Task<string> GenerateInviteCodeAsync(long realmId, CancellationToken cancellationToken = default);
}

public interface IJavaRealmService : IRealmService<JavaRealm, string, SlotOptionsDto>
{
    Task UpdateSlotOptionsAsync(long realmId, int slotId, SlotOptionsDto options,
        CancellationToken cancellationToken = default);
    
    Task OpAsync(long realmId, string player, CancellationToken cancellationToken = default);

    Task DeopAsync(long realmId, string player, CancellationToken cancellationToken = default);
}