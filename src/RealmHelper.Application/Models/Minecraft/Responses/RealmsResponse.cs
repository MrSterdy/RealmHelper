namespace RealmHelper.Application.Models.Minecraft.Responses;

public class RealmsResponse<TRealm> where TRealm : BaseRealmResponse
{
    public TRealm[] Servers { get; set; } = default!;
}

public class BaseRealmResponse
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

public class RealmResponse<TPlayer, TSlot, TSlotOptions> : BaseRealmResponse
    where TPlayer : PlayerResponse
    where TSlot : SlotResponse<TSlotOptions>
    where TSlotOptions : SlotOptionsDto
{
    public TPlayer[]? Players { get; set; }

    public TSlot[]? Slots { get; set; }
}

public class BedrockRealmResponse : RealmResponse<BedrockPlayerResponse, BedrockSlotResponse, BedrockSlotOptionsDto>
{
    public long ClubId { get; set; }
    
    public string DefaultPermission { get; set; } = default!;
    
    public int MaxPlayers { get; set; }
}

public class JavaRealmResponse : RealmResponse<JavaPlayerResponse, JavaSlotResponse, SlotOptionsDto>
{
}