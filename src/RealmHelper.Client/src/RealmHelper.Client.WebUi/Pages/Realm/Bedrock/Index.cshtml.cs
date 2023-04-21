using Microsoft.AspNetCore.Mvc;

using RealmHelper.Client.Application.Services;
using RealmHelper.Client.Domain.Models.XboxLive;
using RealmHelper.Realm.Bedrock.Abstractions.Services;

namespace RealmHelper.Client.WebUi.Pages.Realm.Bedrock;

public class Index : BedrockRealmModel
{
    private readonly IProfileService _profileService;
    
    public Profile Owner { get; set; } = default!;

    public ClubActivity[] Activities { get; set; } = default!;

    public Index(IProfileService profileService, IBedrockRealmService realmService, IClubService clubService) 
        : base(realmService, clubService) =>
        _profileService = profileService;

    public override async Task<IActionResult> OnGet(long realmId, CancellationToken cancellationToken)
    {
        var result = await base.OnGet(realmId, cancellationToken);

#pragma warning disable CS8600
        (Owner, Activities) = await (
            _profileService.GetProfileByXuidAsync(Realm.OwnerUuid, cancellationToken),
            ClubService.GetClubActivitiesAsync(Realm.ClubId, Constants.PageSize, cancellationToken)
        );
#pragma warning restore CS8600

        return result;
    }
}