using RealmHelper.Realm.Common.Application.Models.Responses;

namespace RealmHelper.Realm.Bedrock.Application.Models.Responses;

public class BedrockPlayerResponse : PlayerResponse
{
    public string Permission { get; set; } = default!;
}