using RealmHelper.Domain.Models.Minecraft;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Web;

namespace RealmHelper.Web.Pages.Realm.Shared;

[Authorize]
[AuthorizeForScopes]
public abstract class BaseRealmModel : PageModel
{
    public BaseRealm Realm { get; set; } = default!;

    public virtual async Task<IActionResult> OnGet(long realmId, CancellationToken cancellationToken)
    {
        Realm = await FetchRealm(realmId, cancellationToken);

        return Page();
    }

    protected abstract Task<BaseRealm> FetchRealm(long realmId, CancellationToken cancellationToken);
}