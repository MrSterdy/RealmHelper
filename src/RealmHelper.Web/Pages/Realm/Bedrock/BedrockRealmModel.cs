using RealmHelper.Application.Services;
using RealmHelper.Domain.Models.Minecraft;
using RealmHelper.Domain.Models.Minecraft.Bedrock;
using RealmHelper.Web.Pages.Realm.Shared;

namespace RealmHelper.Web.Pages.Realm.Bedrock;

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