using System.Text.Json.Serialization;

namespace RealmHelper.Application.Models.Minecraft.Responses;

public class JavaAuthenticationResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = default!;
}