namespace RealmHelper.Application.Models.Minecraft.Requests;

public class SlotUpdateRequest<TOptions> where TOptions : SlotOptionsDto
{
    public DescriptionUpdateRequest Description { get; set; } = default!;

    public TOptions Options { get; set; } = default!;
}