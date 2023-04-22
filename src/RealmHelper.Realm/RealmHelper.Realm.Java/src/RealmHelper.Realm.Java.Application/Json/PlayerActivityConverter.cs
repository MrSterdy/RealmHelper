using System.Text.Json;
using System.Text.Json.Serialization;

namespace RealmHelper.Realm.Java.Application.Json;

public class PlayerActivityConverter : JsonConverter<string[]>
{
    public override string[]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        JsonSerializer.Deserialize<string[]>(reader.GetString()!, options);

    public override void Write(Utf8JsonWriter writer, string[] value, JsonSerializerOptions options) =>
        writer.WriteStringValue(JsonSerializer.Serialize(value, options));
}