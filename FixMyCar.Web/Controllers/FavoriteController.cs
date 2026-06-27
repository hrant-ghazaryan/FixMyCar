using FixMyCar.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FixMyCar.Web.Controllers;

[Authorize]
public class FavoriteController(IFavoriteService favoriteService) : Controller
{
    private readonly IFavoriteService _favoriteService = favoriteService;
    [Authorize]
    public async Task<IActionResult> Index()
    {
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!
        );

        var favorites = await _favoriteService
            .GetUserFavoritesAsync(userId);

        return View(favorites);
    }

    [HttpPost]
    public async Task<IActionResult> Toggle(int postId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        await _favoriteService.ToggleAsync(userId, postId);

        var referer = Request.Headers["Referer"].ToString();

        if (!string.IsNullOrWhiteSpace(referer))
        {
            return Redirect(referer);
        }

        return RedirectToAction("Index", "Home");
    }

}
