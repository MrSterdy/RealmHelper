using Microsoft.AspNetCore.Mvc;

using RealmHelper.Realm.Bedrock.Abstractions.Models;
using RealmHelper.Realm.Bedrock.Abstractions.Services;
using RealmHelper.Realm.Common.Abstractions.Models;

namespace RealmHelper.Client.WebUi.Pages.Realm.Bedrock;

public class Settings : OwnedBedrockRealmModel
{
    public Slot<BedrockSlotOptions> ActiveSlot { get; set; } = default!;

    public string InviteCode { get; set; } = default!;
    
    public Settings(IBedrockRealmService realmService) : base(realmService)
    {
    }

    public override async Task<IActionResult> OnGet(long realmId, CancellationToken cancellationToken)
    {
        var result = await base.OnGet(realmId, cancellationToken);

        ActiveSlot = Realm.Slots!.First(s => s.SlotId == Realm.ActiveSlot);
        InviteCode = await RealmService.GetInviteCodeAsync(realmId, cancellationToken);

        return result;
    }
}