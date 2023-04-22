using Microsoft.AspNetCore.Mvc;

using RealmHelper.Client.Application.Services;
using RealmHelper.Client.Domain.Models.XboxLive;
using RealmHelper.Client.Infrastructure.Utils;
using RealmHelper.Realm.Bedrock.Abstractions.Models;
using RealmHelper.Realm.Bedrock.Abstractions.Services;
using RealmHelper.Realm.Common.Abstractions.Models;

namespace RealmHelper.Client.WebUi.Pages.Realm.Bedrock;

public class Member : BedrockRealmModel
{
    private readonly IProfileService _profileService;
    private readonly IClubService _clubService;

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

    public Member(IProfileService profileService, IBedrockRealmService realmService, IClubService clubService)
        : base(realmService) =>
        (_profileService, _clubService) = (profileService, clubService);

    public override async Task<IActionResult> OnGet(long realmId, CancellationToken cancellationToken)
    {
        await base.OnGet(realmId, cancellationToken);

        (Profile, Club) = await (long.TryParse(MemberId, out _)
                ? _profileService.GetProfileByXuidAsync(MemberId, cancellationToken)
                : _profileService.GetProfileByGamertagAsync(MemberId, cancellationToken),
            _clubService.GetClubAsync(Realm.ClubId, cancellationToken));

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