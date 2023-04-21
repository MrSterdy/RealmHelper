namespace RealmHelper.Realm.Common.Abstractions.Models;

public class BaseRealm
{
    public long Id { get; set; }

    public string OwnerUuid { get; set; } = default!;

    public string Name { get; set; } = default!;
    public string Motd { get; set; } = default!;

    public string State { get; set; } = default!;
    
    public int ActiveSlot { get; set; }
    
    public bool Expired { get; set; }
    public int DaysLeft { get; set; }
}

public class Realm<TPlayer, TSlotOptions> : BaseRealm
    where TPlayer : Player
    where TSlotOptions : SlotOptions
{
    public TPlayer[]? Players { get; set; }
    
    public Slot<TSlotOptions>[]? Slots { get; set; }
}