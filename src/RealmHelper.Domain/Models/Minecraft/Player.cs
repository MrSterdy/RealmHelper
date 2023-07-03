namespace RealmHelper.Domain.Models.Minecraft;

public class Player
{
    public string Uuid { get; set; } = default!;
    
    public bool Accepted { get; set; }
    public bool Online { get; set; }
    public bool Operator { get; set; }
}