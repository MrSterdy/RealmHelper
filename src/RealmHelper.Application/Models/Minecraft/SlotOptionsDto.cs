using System.Text.Json.Serialization;

namespace RealmHelper.Application.Models.Minecraft;

public class SlotOptionsDto
{
    public string SlotName { get; set; } = default!;
    
    public bool Pvp { get; set; }
    
    public bool SpawnAnimals { get; set; }
    public bool SpawnMonsters { get; set; }
    [JsonPropertyName("spawnNPCs")]
    public bool SpawnNpcs { get; set; }
    
    public int SpawnProtection { get; set; }
    
    public bool CommandBlocks { get; set; }
    
    public bool ForceGameMode { get; set; }
    public int GameMode { get; set; }
    
    public int Difficulty { get; set; }
    
    public int WorldTemplateId { get; set; }
    public string? WorldTemplateImage { get; set; }
}

public class BedrockSlotOptionsDto : SlotOptionsDto
{
    public class Packs
    {
        public string[] ResourcePacks { get; set; } = default!;
        public string[] BehaviorPacks { get; set; } = default!;
    }
    
    public class WorldSetting
    {
        public int Type { get; set; }

        public object Value { get; set; } = default!;
    }
    
    public bool AdventureMap { get; set; }
    
    public bool TexturePacksRequired { get; set; }

    public Packs EnabledPacks { get; set; } = default!;

    public bool CheatsAllowed { get; set; }

    public Dictionary<string, WorldSetting> WorldSettings { get; set; } = default!;
}