using Microsoft.AspNetCore.Mvc;

using RealmHelper.Client.Application.Services;
using RealmHelper.Client.Domain.Models.XboxLive;
using RealmHelper.Client.WebUi.Pages.Realm.Shared;
using RealmHelper.Realm.Bedrock.Abstractions.Models;
using RealmHelper.Realm.Bedrock.Abstractions.Services;
using RealmHelper.Realm.Common.Abstractions.Models;

namespace RealmHelper.Client.WebUi.Pages.Realm.Bedrock;

public abstract class BedrockRealmModel : BaseRealmModel
{
    protected readonly IBedrockRealmService RealmService;
    protected readonly IClubService ClubService;

    public new BedrockRealm Realm
    {
        get => (BedrockRealm)base.Realm;
        set => base.Realm = value;
    }
    
    public Club Club { get; set; } = default!;

    public BedrockRealmModel(IBedrockRealmService realmService, IClubService clubService) =>
        (RealmService, ClubService) = (realmService, clubService);
    
    public override async Task<IActionResult> OnGet(long realmId, CancellationToken cancellationToken)
    {
        var result = await base.OnGet(realmId, cancellationToken);

        Club = await FetchClub(Realm.ClubId, cancellationToken);

        return result;
    }

    protected override async Task<BaseRealm> FetchRealm(long realmId, CancellationToken cancellationToken) =>
        (await RealmService.GetRealmsAsync(cancellationToken)).First(r => r.Id == realmId);

    protected virtual async Task<Club> FetchClub(long clubId, CancellationToken cancellationToken) =>
        await ClubService.GetClubAsync(clubId);
}

public abstract class OwnedBedrockRealmModel : BedrockRealmModel
{
    protected OwnedBedrockRealmModel(IBedrockRealmService realmService, IClubService clubService) : base(realmService,
        clubService)
    {
    }

    protected override async Task<BaseRealm> FetchRealm(long realmId, CancellationToken cancellationToken) =>
        await RealmService.GetRealmAsync(realmId, cancellationToken);
}