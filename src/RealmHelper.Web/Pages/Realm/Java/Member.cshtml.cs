using Microsoft.AspNetCore.Mvc;

using RealmHelper.Application.Services;
using RealmHelper.Application.Utils;
using RealmHelper.Domain.Models.Minecraft;
using RealmHelper.Domain.Models.Minecraft.Java;
using RealmHelper.Infrastructure.Utils;

namespace RealmHelper.Web.Pages.Realm.Java;

public class Member : JavaRealmModel
{
    [BindProperty(SupportsGet = true)] 
    public string MemberId { get; set; } = default!;

    public JavaPlayer Player { get; set; } = default!;
    
    public bool IsInRealm { get; set; }
    public bool IsRealmOwned { get; set; }
    public bool IsOwner { get; set; }
    public bool IsInvited { get; set; }
    public bool IsOnline { get; set; }

    public Member(IJavaRealmService realmService) : base(realmService)
    {
    }

    public override async Task<IActionResult> OnGet(long realmId, CancellationToken cancellationToken)
    {
        await base.OnGet(realmId, cancellationToken);
        
        var player = Realm.Players?.FirstOrDefault(p => p.Uuid == MemberId || p.Name == MemberId);
        if (player is null)
        {
            var playerData = await PlayerHelper.FetchPlayer(MemberId, cancellationToken);
            Player = new JavaPlayer { Uuid = playerData.Uuid, Name = playerData.Name };
            IsOwner = Player.Uuid == Realm.OwnerUuid;
        }
        else
        {
            Player = player;
            IsInRealm = true;
            IsOnline = (await RealmService.GetOnlinePlayersAsync(realmId, cancellationToken))
                .Any(p => p == Player.Name);
        }

        IsInvited = !Player.Accepted && IsInRealm;

        return Page();
    }

    protected override async Task<BaseRealm> FetchRealm(long realmId, CancellationToken cancellationToken)
    {
        var realm = await base.FetchRealm(realmId, cancellationToken);

        IsRealmOwned = realm.IsOwner(User);

        return IsRealmOwned ? await RealmService.GetRealmAsync(realmId, cancellationToken) : realm;
    }
}