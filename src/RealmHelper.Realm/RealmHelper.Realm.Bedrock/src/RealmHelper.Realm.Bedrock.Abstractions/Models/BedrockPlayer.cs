using RealmHelper.Realm.Common.Abstractions.Models;

namespace RealmHelper.Realm.Bedrock.Abstractions.Models;

public class BedrockPlayer : Player
{
    public string Permission { get; set; } = default!;
}