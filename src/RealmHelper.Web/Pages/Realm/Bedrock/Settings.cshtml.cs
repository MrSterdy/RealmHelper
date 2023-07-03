using Microsoft.AspNetCore.Mvc;

using RealmHelper.Application.Services;
using RealmHelper.Domain.Models.Minecraft;
using RealmHelper.Domain.Models.Minecraft.Bedrock;

namespace RealmHelper.Web.Pages.Realm.Bedrock;

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