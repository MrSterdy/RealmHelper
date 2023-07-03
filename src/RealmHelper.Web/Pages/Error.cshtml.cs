using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RealmHelper.Web.Pages;

public class Error : PageModel
{
    public void OnGet()
    {
        var handler = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

        if (handler?.Error is HttpRequestException error)
            Response.StatusCode = (int)error.StatusCode!;
    }
}