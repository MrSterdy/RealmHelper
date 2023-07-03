using System.Text.Json.Serialization;

using RealmHelper.Application.Json;

namespace RealmHelper.Application.Models.Minecraft.Responses;

public class PlayerActivitiesResponse<TPlayer, TActivityResponse> where TActivityResponse : PlayerActivityResponse<TPlayer>
{
    public virtual TActivityResponse[] Servers { get; set; } = default!;
}

public class PlayerActivityResponse<TPlayer>
{
    public virtual long RealmId { get; set; }

    public virtual TPlayer[] Players { get; set; } = default!;
}

public class BedrockPlayerActivityResponse : PlayerActivityResponse<BedrockPlayerResponse>
{
    [JsonPropertyName("id")]
    public override long RealmId { get; set; }
}

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