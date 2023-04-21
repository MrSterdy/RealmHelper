using RealmHelper.Realm.Common.Abstractions.Types;

namespace RealmHelper.Realm.Common.Abstractions.Models;

public class Slot<TSlotOptions> where TSlotOptions : SlotOptions
{
    public int SlotId { get; set; }

    public TSlotOptions Options { get; set; } = default!;
}

public class SlotOptions
{
    public string SlotName { get; set; } = default!;
    
    public bool Pvp { get; set; }
    
    public bool SpawnAnimals { get; set; }
    public bool SpawnMonsters { get; set; }
    public bool SpawnNpcs { get; set; }
    
    public int SpawnProtection { get; set; }
    
    public bool CommandBlocks { get; set; }
    
    public bool ForceGameMode { get; set; }
    public GameMode GameMode { get; set; }
    
    public Difficulty Difficulty { get; set; }
    
    public int WorldTemplateId { get; set; }
    public string? WorldTemplateImage { get; set; }
}