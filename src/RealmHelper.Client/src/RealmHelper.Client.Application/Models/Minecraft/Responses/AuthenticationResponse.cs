using System.Text.Json.Serialization;

namespace RealmHelper.Client.Application.Models.Minecraft.Responses;

public class AuthenticationResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = default!;
}