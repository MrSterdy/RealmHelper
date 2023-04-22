using Microsoft.AspNetCore.Mvc;

using RealmHelper.Client.Application.Services;
using RealmHelper.Client.Domain.Models.XboxLive;
using RealmHelper.Realm.Bedrock.Abstractions.Services;

namespace RealmHelper.Client.WebUi.Pages.Realm.Bedrock;

public class Index : BedrockRealmModel
{
    private readonly IProfileService _profileService;
    private readonly IClubService _clubService;

    public Club Club { get; set; } = default!;
    
    public Profile Owner { get; set; } = default!;

    public ClubActivity[] Activities { get; set; } = default!;

    public Index(IProfileService profileService, IBedrockRealmService realmService, IClubService clubService) 
        : base(realmService) =>
        (_profileService, _clubService) = (profileService, clubService);

    public override async Task<IActionResult> OnGet(long realmId, CancellationToken cancellationToken)
    {
        var result = await base.OnGet(realmId, cancellationToken);
        
        (Owner, Club, Activities) = await (
            _profileService.GetProfileByXuidAsync(Realm.OwnerUuid, cancellationToken),
            _clubService.GetClubAsync(Realm.ClubId, cancellationToken),
            _clubService.GetClubActivitiesAsync(Realm.ClubId, Constants.PageSize, cancellationToken)
        );

        return result;
    }
}