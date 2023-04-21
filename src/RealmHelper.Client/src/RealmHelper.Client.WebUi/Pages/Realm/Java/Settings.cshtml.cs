using Microsoft.AspNetCore.Mvc;

using RealmHelper.Realm.Common.Abstractions.Models;
using RealmHelper.Realm.Java.Abstractions.Services;

namespace RealmHelper.Client.WebUi.Pages.Realm.Java;

public class Settings : OwnedJavaRealmModel
{
    public Slot<SlotOptions> ActiveSlot { get; set; } = default!;

    public Settings(IJavaRealmService realmService) : base(realmService)
    {
    }

    public override async Task<IActionResult> OnGet(long realmId, CancellationToken cancellationToken)
    {
        var result = await base.OnGet(realmId, cancellationToken);

        ActiveSlot = Realm.Slots!.First(s => s.SlotId == Realm.ActiveSlot);

        return result;
    }
}