using Microsoft.AspNetCore.Mvc;

using RealmHelper.Client.Infrastructure.Utils;
using RealmHelper.Realm.Common.Abstractions.Models;
using RealmHelper.Realm.Java.Abstractions.Models;
using RealmHelper.Realm.Java.Abstractions.Services;

namespace RealmHelper.Client.WebUi.Pages.Realm.Java;

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