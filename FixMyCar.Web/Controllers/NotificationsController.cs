using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FixMyCar.Web.Controllers;

[Authorize]
public class NotificationsController : Controller
{
    // GET: /notifications
    public IActionResult Index()
    {
        return View();
    }
}
