using Microsoft.AspNetCore.Mvc;

using RealmHelper.Application.Services;
using RealmHelper.Domain.Models.XboxLive;

namespace RealmHelper.Web.Pages.Realm.Bedrock;

public class Index : BedrockRealmModel
{
    private readonly IXboxLiveService _xblService;

    public Club Club { get; set; } = default!;
    
    public Profile Owner { get; set; } = default!;

    public ClubActivity[] Activities { get; set; } = default!;

    public Index(IXboxLiveService xblService, IBedrockRealmService realmService) : base(realmService) =>
        _xblService = xblService;

    public override async Task<IActionResult> OnGet(long realmId, CancellationToken cancellationToken)
    {
        var result = await base.OnGet(realmId, cancellationToken);
        
        (Owner, Club, Activities) = await (
            _xblService.GetProfileByXuidAsync(Realm.OwnerUuid, cancellationToken),
            _xblService.GetClubAsync(Realm.ClubId, cancellationToken),
            _xblService.GetClubActivitiesAsync(Realm.ClubId, Constants.PageSize, cancellationToken)
        );

        return result;
    }
}