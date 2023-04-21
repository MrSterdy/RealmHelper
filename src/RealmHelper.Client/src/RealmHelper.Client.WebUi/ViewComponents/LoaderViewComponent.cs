using Microsoft.AspNetCore.Mvc;

namespace RealmHelper.Client.WebUi.ViewComponents;

public class LoaderViewComponent : ViewComponent
{
    public IViewComponentResult Invoke() =>
        View();
}