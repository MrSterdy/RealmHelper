namespace RealmHelper.Application.Models.Minecraft.Responses;

public class PlayerResponse
{
    public string Uuid { get; set; } = default!;
    
    public bool Accepted { get; set; }
    public bool Online { get; set; }
    public bool Operator { get; set; }
}

public class BedrockPlayerResponse : PlayerResponse
{
    public string Permission { get; set; } = default!;
}

public class JavaPlayerResponse : PlayerResponse
{
    public string Name { get; set; } = default!;
}