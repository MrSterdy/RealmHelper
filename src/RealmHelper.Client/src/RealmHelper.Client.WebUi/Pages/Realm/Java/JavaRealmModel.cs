using Microsoft.AspNetCore.Mvc;

using RealmHelper.Client.WebUi.Pages.Realm.Shared;
using RealmHelper.Realm.Common.Abstractions.Models;
using RealmHelper.Realm.Java.Abstractions.Models;
using RealmHelper.Realm.Java.Abstractions.Services;

namespace RealmHelper.Client.WebUi.Pages.Realm.Java;

public abstract class JavaRealmModel : BaseRealmModel
{
    protected readonly IJavaRealmService RealmService;

    public new JavaRealm Realm
    {
        get => (JavaRealm)base.Realm;
        set => base.Realm = value;
    }

    public JavaRealmModel(IJavaRealmService realmService) =>
        RealmService = realmService;
    
    public override async Task<IActionResult> OnGet(long realmId, CancellationToken cancellationToken)
    {
        var result = await base.OnGet(realmId, cancellationToken);

        return result;
    }

    protected override async Task<BaseRealm> FetchRealm(long realmId, CancellationToken cancellationToken) =>
        (await RealmService.GetRealmsAsync(cancellationToken)).First(r => r.Id == realmId);
}

public abstract class OwnedJavaRealmModel : JavaRealmModel
{
    protected OwnedJavaRealmModel(IJavaRealmService realmService) : base(realmService)
    {
    }

    protected override async Task<BaseRealm> FetchRealm(long realmId, CancellationToken cancellationToken) =>
        await RealmService.GetRealmAsync(realmId, cancellationToken);
}