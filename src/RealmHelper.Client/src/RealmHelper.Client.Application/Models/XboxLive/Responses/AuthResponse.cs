using System.Text.Json.Serialization;

namespace RealmHelper.Client.Application.Models.XboxLive.Responses;

public class AuthResponse
{
    public class Claims
    {
        public class Hash
        {
            [JsonPropertyName("uhs")]
            public string Uhs { get; set; } = default!;
        }

        [JsonPropertyName("xui")]
        public Hash[] Xui { get; set; } = default!;
    }
    
    public string Token { get; set; } = default!;
    
    public Claims DisplayClaims { get; set; } = default!;
}