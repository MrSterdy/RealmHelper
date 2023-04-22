using System.Text.Json.Serialization;

using RealmHelper.Realm.Common.Application.Models.Responses;

namespace RealmHelper.Realm.Bedrock.Application.Models.Responses;

public class BedrockPlayerActivityResponse : PlayerActivityResponse<BedrockPlayerResponse>
{
    [JsonPropertyName("id")]
    public override long RealmId { get; set; }
}