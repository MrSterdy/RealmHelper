using RealmHelper.Realm.Bedrock.Abstractions.Models;
using RealmHelper.Realm.Common.Abstractions.Services;

namespace RealmHelper.Realm.Bedrock.Abstractions.Services;

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