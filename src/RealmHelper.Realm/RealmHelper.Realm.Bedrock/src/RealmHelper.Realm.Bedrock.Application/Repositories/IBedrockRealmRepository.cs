using RealmHelper.Realm.Bedrock.Abstractions.Models;
using RealmHelper.Realm.Bedrock.Application.Models.Requests;
using RealmHelper.Realm.Bedrock.Application.Models.Responses;
using RealmHelper.Realm.Common.Application.Repositories;

namespace RealmHelper.Realm.Bedrock.Application.Repositories;

public interface IBedrockRealmRepository : IRealmRepository<
    BedrockRealmResponse, 
    BedrockPlayerResponse,
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