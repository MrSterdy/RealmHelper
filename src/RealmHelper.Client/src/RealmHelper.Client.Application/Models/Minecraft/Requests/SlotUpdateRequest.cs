using RealmHelper.Realm.Common.Abstractions.Models;

namespace RealmHelper.Client.Application.Models.Minecraft.Requests;

public class SlotUpdateRequest<TSlotOptions> where TSlotOptions : SlotOptionsDto
{
    public DescriptionUpdateRequest Description { get; set; } = default!;

    public TSlotOptions Options { get; set; } = default!;
}

public class DescriptionUpdateRequest
{
    public string Name { get; set; } = default!;

    public string Description { get; set; } = default!;
}