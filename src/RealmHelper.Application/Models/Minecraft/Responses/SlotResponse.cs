using System.Text.Json.Serialization;

using RealmHelper.Application.Json;

namespace RealmHelper.Application.Models.Minecraft.Responses;

public class SlotResponse<TOptions> where TOptions : SlotOptionsDto
{
    public int SlotId { get; set; }

    public virtual TOptions Options { get; set; } = default!;
}

public class BedrockSlotResponse : SlotResponse<BedrockSlotOptionsDto>
{
    [JsonConverter(typeof(BedrockSlotOptionsResponseConverter))]
    public override BedrockSlotOptionsDto Options { get; set; } = default!;
}

public class JavaSlotResponse : SlotResponse<SlotOptionsDto>
{
    [JsonConverter(typeof(SlotOptionsDtoConverter))]
    public override SlotOptionsDto Options { get; set; } = default!;
}