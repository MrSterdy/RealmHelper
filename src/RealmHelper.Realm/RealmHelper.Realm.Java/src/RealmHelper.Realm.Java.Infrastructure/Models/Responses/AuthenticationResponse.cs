using System.Text.Json.Serialization;

namespace RealmHelper.Realm.Java.Infrastructure.Models.Responses;

public class AuthenticationResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = default!;
}