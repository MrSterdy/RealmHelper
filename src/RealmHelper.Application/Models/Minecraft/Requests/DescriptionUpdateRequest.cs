namespace RealmHelper.Application.Models.Minecraft.Requests;

public class DescriptionUpdateRequest
{
    public string Name { get; set; } = default!;

    public string Description { get; set; } = default!;
}