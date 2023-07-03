using RealmHelper.Application.Models.Minecraft;
using RealmHelper.Application.Models.Minecraft.Requests;
using RealmHelper.Application.Models.Minecraft.Responses;

namespace RealmHelper.Application.Api;

public interface IRealmApi<TRealm, TActivityPlayer, TActivityResponse, TSlotOptions>
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

public interface IBedrockRealmApi : IRealmApi<
    BedrockRealmResponse, 
    BedrockPlayerResponse,
    BedrockPlayerActivityResponse,
    BedrockSlotOptionsDto
>
{
    Task<string[]> GetBlockedPlayersAsync(long realmId, CancellationToken cancellationToken = default);

    Task BlockPlayerAsync(long realmId, string xuid, CancellationToken cancellationToken = default);

    Task UnblockPlayerAsync(long realmId, string xuid, CancellationToken cancellationToken = default);

    Task ChangePlayerPermissionAsync(long realmId, PlayerPermissionChangeRequest request,
        CancellationToken cancellationToken = default);

    Task ChangeDefaultPermissionAsync(long realmId, PermissionChangeRequest request,
        CancellationToken cancellationToken = default);

    Task<InviteLinkResponse[]> GetInviteCodesAsync(long realmId, CancellationToken cancellationToken = default);

    Task<InviteLinkResponse> GenerateInviteCodeAsync(InviteLinkRequest request,
        CancellationToken cancellationToken = default);
}

public interface IJavaRealmApi : IRealmApi<JavaRealmResponse, string, JavaPlayerActivityResponse, SlotOptionsDto>
{
    Task UpdateSlotAsync(long realmId, int slotId, SlotOptionsDto options, CancellationToken cancellationToken = default);
    
    Task OpAsync(long realmId, string player, CancellationToken cancellationToken = default);

    Task DeopAsync(long realmId, string player, CancellationToken cancellationToken = default);
}