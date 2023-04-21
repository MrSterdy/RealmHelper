using RealmHelper.Realm.Common.Abstractions.Models;

namespace RealmHelper.Realm.Java.Abstractions.Models;

public class JavaRealm : Realm<JavaPlayer, SlotOptions>
{
    public string Owner { get; set; } = default!;
}