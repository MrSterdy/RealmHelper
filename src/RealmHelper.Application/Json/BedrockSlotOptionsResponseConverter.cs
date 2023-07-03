using System.Text.Json;
using System.Text.Json.Serialization;

using RealmHelper.Application.Models.Minecraft;

namespace RealmHelper.Application.Json;

public class BedrockSlotOptionsResponseConverter : JsonConverter<BedrockSlotOptionsDto>
{
    public override BedrockSlotOptionsDto? Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options) =>
        JsonSerializer.Deserialize<BedrockSlotOptionsDto>(reader.GetString()!, options);

    public override void Write(Utf8JsonWriter writer, BedrockSlotOptionsDto value,
        JsonSerializerOptions options) =>
        writer.WriteStringValue(JsonSerializer.Serialize(value, options));
}