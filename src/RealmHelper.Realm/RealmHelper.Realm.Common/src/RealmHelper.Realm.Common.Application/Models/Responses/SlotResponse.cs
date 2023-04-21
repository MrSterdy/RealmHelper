using RealmHelper.Realm.Common.Abstractions.Models;

namespace RealmHelper.Realm.Common.Application.Models.Responses;

public class SlotResponse<TOptions> where TOptions : SlotOptionsDto
{
    public int SlotId { get; set; }

    public virtual TOptions Options { get; set; } = default!;
}