namespace RealmHelper.Domain.Models.Minecraft.Bedrock;

public class BedrockRealm : Realm<BedrockPlayer, BedrockSlotOptions>
{
    public long ClubId { get; set; }

    public string DefaultPermission { get; set; } = default!;

    public int MaxPlayers { get; set; }
}