using FixMyCar.Web.Data;
using FixMyCar.Web.Models;
using FixMyCar.Web.Services;
using FixMyCar.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Diagnostics;
using System.Diagnostics;
using System.Security.Claims;

namespace FixMyCar.Web.Controllers;

public class HomeController(ICategoryService categoryService, IPostService postService,
    IFavoriteService favoriteService, ILogger<HomeController> logger) : Controller
{
    private readonly ICategoryService _categoryService = categoryService;
    private readonly IPostService _postService = postService;
    private readonly IFavoriteService _favoriteService = favoriteService;
    private readonly ILogger<HomeController> _logger = logger;

    public async Task<IActionResult> Index(int page = 1, string? search = null)
    {
        int pageSize = 6;

        // total count (with search support)
        var totalPosts = await _postService.GetCountAsync(search);

        // paged posts (with search support)
        var posts = await _postService.GetPagedAsync(page, pageSize, search);

        // categories
        var categories = await _categoryService.GetAllAsync();

        List<int> favoritePostIds = new();

        if (User.Identity!.IsAuthenticated)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            favoritePostIds = await _favoriteService.GetFavoritePostIdsAsync(userId);
        }

        var vm = new HomeViewModel
        {
            Posts = posts,
            Categories = categories,
            Pagination = new PaginationViewModel
            {
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalPosts / (double)pageSize),
                BaseUrl = Url.Action("Index", "Home") ?? "/"
            },
            FavoritePostIds = favoritePostIds,
        };

        return View(vm);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerFeature>();
        if (exceptionFeature?.Error != null)
        {
            _logger.LogError(exceptionFeature.Error,
                "Unhandled request failure. Trace identifier: {TraceIdentifier}",
                HttpContext.TraceIdentifier);
        }

        Response.StatusCode = StatusCodes.Status500InternalServerError;
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult StatusCodePage(int statusCode)
    {
        Response.StatusCode = statusCode;
        return View("Error", new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
    }
}
