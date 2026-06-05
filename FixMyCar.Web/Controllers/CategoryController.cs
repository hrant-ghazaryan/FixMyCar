using Microsoft.AspNetCore.Mvc;

namespace FixMyCar.Web.Controllers;

public class CategoryController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
