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
        : base(realmService, clubService) =>
        _profileService = profileService;

    public override async Task<IActionResult> OnGet(long realmId, CancellationToken cancellationToken)
    {
        await base.OnGet(realmId, cancellationToken);

        var profile = long.TryParse(MemberId, out _)
            ? await _profileService.GetProfileByXuidAsync(MemberId, cancellationToken)
            : await _profileService.GetProfileByGamertagAsync(MemberId, cancellationToken);

        Profile = profile;

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