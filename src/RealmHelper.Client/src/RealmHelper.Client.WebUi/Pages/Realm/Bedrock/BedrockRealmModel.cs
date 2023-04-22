using RealmHelper.Client.WebUi.Pages.Realm.Shared;
using RealmHelper.Realm.Bedrock.Abstractions.Models;
using RealmHelper.Realm.Bedrock.Abstractions.Services;
using RealmHelper.Realm.Common.Abstractions.Models;

namespace RealmHelper.Client.WebUi.Pages.Realm.Bedrock;

public abstract class BedrockRealmModel : BaseRealmModel
{
    protected readonly IBedrockRealmService RealmService;

    public new BedrockRealm Realm
    {
        get => (BedrockRealm)base.Realm;
        set => base.Realm = value;
    }

    protected BedrockRealmModel(IBedrockRealmService realmService) =>
        RealmService = realmService;

    protected override async Task<BaseRealm> FetchRealm(long realmId, CancellationToken cancellationToken) =>
        (await RealmService.GetRealmsAsync(cancellationToken)).First(r => r.Id == realmId);
}

public abstract class OwnedBedrockRealmModel : BedrockRealmModel
{
    protected OwnedBedrockRealmModel(IBedrockRealmService realmService) : base(realmService)
    {
    }

    protected override async Task<BaseRealm> FetchRealm(long realmId, CancellationToken cancellationToken) =>
        await RealmService.GetRealmAsync(realmId, cancellationToken);
}