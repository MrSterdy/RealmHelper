using RealmHelper.Realm.Common.Abstractions.Models;

namespace RealmHelper.Realm.Common.Application.Models.Requests;

public class SlotUpdateRequest<TOptions> where TOptions : SlotOptionsDto
{
    public DescriptionUpdateRequest Description { get; set; } = default!;

    public TOptions Options { get; set; } = default!;
}