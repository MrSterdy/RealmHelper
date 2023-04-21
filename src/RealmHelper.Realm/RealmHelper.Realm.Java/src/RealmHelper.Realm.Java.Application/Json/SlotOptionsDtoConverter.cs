using System.Text.Json;
using System.Text.Json.Serialization;

using RealmHelper.Realm.Common.Abstractions.Models;

namespace RealmHelper.Realm.Java.Application.Json;

public class SlotOptionsDtoConverter : JsonConverter<SlotOptionsDto>
{
    public override SlotOptionsDto?
        Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        JsonSerializer.Deserialize<SlotOptionsDto?>(reader.GetString()!, options);

    public override void Write(Utf8JsonWriter writer, SlotOptionsDto value, JsonSerializerOptions options) =>
        writer.WriteStringValue(JsonSerializer.Serialize(value, options));
}