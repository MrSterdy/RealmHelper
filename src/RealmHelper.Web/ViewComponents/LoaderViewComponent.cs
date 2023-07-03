using Microsoft.AspNetCore.Mvc;

namespace RealmHelper.Web.ViewComponents;

public class LoaderViewComponent : ViewComponent
{
    public IViewComponentResult Invoke() =>
        View();
}