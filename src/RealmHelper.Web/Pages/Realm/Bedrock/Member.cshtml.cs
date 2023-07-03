using Microsoft.AspNetCore.Mvc;

using RealmHelper.Application.Services;
using RealmHelper.Application.Utils;
using RealmHelper.Domain.Models.Minecraft;
using RealmHelper.Domain.Models.Minecraft.Bedrock;
using RealmHelper.Domain.Models.XboxLive;

namespace RealmHelper.Web.Pages.Realm.Bedrock;

public class Member : BedrockRealmModel
{
    private readonly IXboxLiveService _xblService;

    public Club Club { get; set; } = default!;

    [BindProperty(SupportsGet = true)] 
    public string MemberId { get; set; } = default!;
    
    public bool OwnedRealm { get; set; }

    public Profile Profile { get; set; } = default!;
    public BedrockPlayer? Player { get; set; }
    public ClubMember? ClubMember { get; set; }
    
    public bool Owner { get; set; }

    public bool Blocked { get; set; }
    public bool Invited { get; set; }

    public Member(IXboxLiveService xblService, IBedrockRealmService realmService) : base(realmService) =>
        _xblService = xblService;

    public override async Task<IActionResult> OnGet(long realmId, CancellationToken cancellationToken)
    {
        await base.OnGet(realmId, cancellationToken);

        (Profile, Club) = await (long.TryParse(MemberId, out _)
                ? _xblService.GetProfileByXuidAsync(MemberId, cancellationToken)
                : _xblService.GetProfileByGamertagAsync(MemberId, cancellationToken),
            _xblService.GetClubAsync(Realm.ClubId, cancellationToken));

        Owner = Profile.Xuid == Realm.OwnerUuid;

        Player = Realm.Players?.FirstOrDefault(p => p.Uuid == Profile.Xuid);

        if (OwnedRealm)
        {
            var blacklist = await RealmService.GetBlockedPlayersAsync(realmId, cancellationToken);
            
            Blocked = blacklist.Contains(Profile.Xuid);
            Invited = !Blocked && Player?.Accepted == false;
        }

        ClubMember = Club.Members.FirstOrDefault(m => m.Xuid == Profile.Xuid);

        return Page();
    }

    protected override async Task<BaseRealm> FetchRealm(long realmId, CancellationToken cancellationToken)
    {
        var realm = await base.FetchRealm(realmId, cancellationToken);

        OwnedRealm = realm.IsOwner(User);
        
        return OwnedRealm ? await RealmService.GetRealmAsync(realmId, cancellationToken) : realm;
    }
}