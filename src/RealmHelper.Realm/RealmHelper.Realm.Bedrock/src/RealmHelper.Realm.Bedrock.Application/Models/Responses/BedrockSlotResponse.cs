using System.Text.Json.Serialization;

using RealmHelper.Realm.Bedrock.Abstractions.Models;
using RealmHelper.Realm.Bedrock.Application.Json;
using RealmHelper.Realm.Common.Application.Models.Responses;

namespace RealmHelper.Realm.Bedrock.Application.Models.Responses;

public class BedrockSlotResponse : SlotResponse<BedrockSlotOptionsDto>
{
    [JsonConverter(typeof(BedrockSlotOptionsResponseConverter))]
    public override BedrockSlotOptionsDto Options { get; set; } = default!;
}