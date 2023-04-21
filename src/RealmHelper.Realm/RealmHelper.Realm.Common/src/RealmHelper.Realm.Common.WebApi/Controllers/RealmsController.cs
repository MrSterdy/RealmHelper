using Microsoft.AspNetCore.Mvc;

using RealmHelper.Realm.Common.Abstractions.Models;

namespace RealmHelper.Realm.Common.WebApi.Controllers;

[Route("[controller]")]
public abstract class RealmsController<TRealm> : BaseController where TRealm : BaseRealm
{
    [HttpGet]
    public abstract Task<ActionResult<TRealm[]>> GetRealms(CancellationToken cancellationToken);
}