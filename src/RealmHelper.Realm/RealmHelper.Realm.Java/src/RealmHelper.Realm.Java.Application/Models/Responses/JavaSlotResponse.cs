using System.Text.Json.Serialization;

using RealmHelper.Realm.Common.Abstractions.Models;
using RealmHelper.Realm.Common.Application.Models.Responses;
using RealmHelper.Realm.Java.Application.Json;

namespace RealmHelper.Realm.Java.Application.Models.Responses;

public class JavaSlotResponse : SlotResponse<SlotOptionsDto>
{
    [JsonConverter(typeof(SlotOptionsDtoConverter))]
    public override SlotOptionsDto Options { get; set; } = default!;
}