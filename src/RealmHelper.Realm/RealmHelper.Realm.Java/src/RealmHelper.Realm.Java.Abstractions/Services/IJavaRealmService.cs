using RealmHelper.Realm.Common.Abstractions.Models;
using RealmHelper.Realm.Common.Abstractions.Services;
using RealmHelper.Realm.Java.Abstractions.Models;

namespace RealmHelper.Realm.Java.Abstractions.Services;

public interface IJavaRealmService : IRealmService<JavaRealm, string, SlotOptionsDto>
{
    Task UpdateSlotOptionsAsync(long realmId, int slotId, SlotOptionsDto options,
        CancellationToken cancellationToken = default);
    
    Task OpAsync(long realmId, string player, CancellationToken cancellationToken = default);

    Task DeopAsync(long realmId, string player, CancellationToken cancellationToken = default);
}