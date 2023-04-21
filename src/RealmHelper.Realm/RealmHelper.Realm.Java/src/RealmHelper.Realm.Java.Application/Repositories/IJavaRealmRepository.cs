using RealmHelper.Realm.Common.Abstractions.Models;
using RealmHelper.Realm.Common.Application.Repositories;
using RealmHelper.Realm.Java.Application.Models.Responses;

namespace RealmHelper.Realm.Java.Application.Repositories;

public interface IJavaRealmRepository : IRealmRepository<JavaRealmResponse, string, SlotOptionsDto>
{
    Task UpdateSlotAsync(long realmId, int slotId, SlotOptionsDto options, CancellationToken cancellationToken = default);
    
    Task OpAsync(long realmId, string player, CancellationToken cancellationToken = default);

    Task DeopAsync(long realmId, string player, CancellationToken cancellationToken = default);
}