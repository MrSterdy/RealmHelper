namespace RealmHelper.Realm.Common.Abstractions.Models;

public class Player
{
    public string Uuid { get; set; } = default!;
    
    public bool Accepted { get; set; }
    public bool Online { get; set; }
    public bool Operator { get; set; }
}