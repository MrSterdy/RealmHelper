using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RealmHelper.Realm.Common.WebApi.Controllers;

[Authorize]
[ApiController]
public abstract class BaseController : ControllerBase
{
}