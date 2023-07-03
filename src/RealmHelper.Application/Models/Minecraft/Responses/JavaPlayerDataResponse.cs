using System.Text.Json.Serialization;

namespace RealmHelper.Application.Models.Minecraft.Responses;

public class JavaPlayerDataResponse
{
    public class DataResponse
    {
        public class PlayerResponse
        {
            public string Username { get; set; } = default!;
        
            [JsonPropertyName("raw_id")]
            public string RawId { get; set; } = default!;
        }

        public PlayerResponse Player { get; set; } = default!;
    }

    public DataResponse Data { get; set; } = default!;
}