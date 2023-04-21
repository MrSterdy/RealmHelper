namespace RealmHelper.Realm.Bedrock.Application.Models.Requests;

public class InviteLinkRequest
{
    public string Type { get; set; } = default!;
    
    public long WorldId { get; set; }
}