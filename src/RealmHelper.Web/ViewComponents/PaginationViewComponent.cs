using Microsoft.AspNetCore.Mvc;
using RealmHelper.Web.Models;

namespace RealmHelper.Web.ViewComponents;

public class PaginationViewComponent : ViewComponent
{
    public IViewComponentResult Invoke(Page page) =>
        View(page);
}