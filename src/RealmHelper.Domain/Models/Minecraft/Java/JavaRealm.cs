namespace RealmHelper.Domain.Models.Minecraft.Java;

public class JavaRealm : Realm<JavaPlayer, SlotOptions>
{
    public string Owner { get; set; } = default!;
}