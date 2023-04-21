using Microsoft.AspNetCore.Mvc;

using RealmHelper.Realm.Common.WebApi.Controllers;
using RealmHelper.Realm.Java.Abstractions.Models;
using RealmHelper.Realm.Java.Abstractions.Services;

namespace RealmHelper.Realm.Java.WebApi.Controllers;

public class RealmsController : RealmsController<JavaRealm>
{
    private readonly IJavaRealmService _realmService;

    public RealmsController(IJavaRealmService realmService) =>
        _realmService = realmService;
    
    public override async Task<ActionResult<JavaRealm[]>> GetRealms(CancellationToken cancellationToken) =>
        Ok(await _realmService.GetRealmsAsync(cancellationToken));
}