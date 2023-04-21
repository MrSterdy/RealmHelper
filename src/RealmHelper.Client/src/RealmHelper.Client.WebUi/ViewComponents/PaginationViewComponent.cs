using Microsoft.AspNetCore.Mvc;
using RealmHelper.Client.WebUi.Models;

namespace RealmHelper.Client.WebUi.ViewComponents;

public class PaginationViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(Page page) =>
        View(page);
}