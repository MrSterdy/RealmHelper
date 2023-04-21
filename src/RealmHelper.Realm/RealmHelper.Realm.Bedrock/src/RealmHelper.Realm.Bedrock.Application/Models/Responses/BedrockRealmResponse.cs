using RealmHelper.Realm.Bedrock.Abstractions.Models;
using RealmHelper.Realm.Common.Application.Models.Responses;

namespace RealmHelper.Realm.Bedrock.Application.Models.Responses;

public class BedrockRealmResponse : RealmResponse<BedrockPlayerResponse, BedrockSlotResponse, BedrockSlotOptionsDto>
{
    public long ClubId { get; set; }
    
    public string DefaultPermission { get; set; } = default!;
    
    public int MaxPlayers { get; set; }
}