namespace RealmHelper.Realm.Common.Application.Models.Responses;

public class PlayerResponse
{
    public string Uuid { get; set; } = default!;
    
    public bool Accepted { get; set; }
    public bool Online { get; set; }
    public bool Operator { get; set; }
}