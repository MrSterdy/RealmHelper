using OneOf;

namespace RealmHelper.Domain.Models.Minecraft.Bedrock;

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

    public Dictionary<string, OneOf<bool, int, object>> WorldSettings { get; set; } = default!;
}