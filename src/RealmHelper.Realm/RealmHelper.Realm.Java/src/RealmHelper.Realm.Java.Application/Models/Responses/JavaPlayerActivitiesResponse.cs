using System.Text.Json.Serialization;

using RealmHelper.Realm.Common.Application.Models.Responses;
using RealmHelper.Realm.Java.Application.Json;

namespace RealmHelper.Realm.Java.Application.Models.Responses;

public class JavaPlayerActivitiesResponse : PlayerActivitiesResponse<string, JavaPlayerActivityResponse>
{
    [JsonPropertyName("lists")]
    public override JavaPlayerActivityResponse[] Servers { get; set; } = default!;
}

public class JavaPlayerActivityResponse : PlayerActivityResponse<string>
{
    [JsonPropertyName("serverId")]
    public override long RealmId { get; set; }

    [JsonPropertyName("playerList")]
    [JsonConverter(typeof(PlayerActivityConverter))]
    public override string[] Players { get; set; } = default!;
}