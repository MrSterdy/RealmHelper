using RealmHelper.Realm.Common.Abstractions.Models;

namespace RealmHelper.Realm.Bedrock.Abstractions.Models;

public class BedrockRealm : Realm<BedrockPlayer, BedrockSlotOptions>
{
    public long ClubId { get; set; }

    public string DefaultPermission { get; set; } = default!;

    public int MaxPlayers { get; set; }
}