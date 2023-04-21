using OneOf;

using RealmHelper.Realm.Common.Abstractions.Models;

namespace RealmHelper.Realm.Bedrock.Abstractions.Models;

public class BedrockSlotOptionsDto : SlotOptionsDto
{
    public class Packs
    {
        public string[] ResourcePacks { get; set; } = default!;
        public string[] BehaviorPacks { get; set; } = default!;
    }
    
    public bool AdventureMap { get; set; }
    
    public bool TexturePacksRequired { get; set; }

    public Packs EnabledPacks { get; set; } = default!;

    public bool CheatsAllowed { get; set; }

    public Dictionary<string, WorldSetting> WorldSettings { get; set; } = default!;
}

public class WorldSetting
{
    public enum SettingType
    {
        Boolean,
        Integer
    }

    public SettingType Type { get; set; }

    public OneOf<bool, int> Value { get; set; } = default!;
}