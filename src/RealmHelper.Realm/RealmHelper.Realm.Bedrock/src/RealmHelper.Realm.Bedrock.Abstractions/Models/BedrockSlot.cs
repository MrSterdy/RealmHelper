using OneOf;

using RealmHelper.Realm.Common.Abstractions.Models;

namespace RealmHelper.Realm.Bedrock.Abstractions.Models;

public class BedrockSlotOptions : SlotOptions
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

    public Dictionary<string, OneOf<bool, int>> WorldSettings { get; set; } = default!;
}