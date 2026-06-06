using FixMyCar.Web.Models;
using FixMyCar.Web.Services;
using FixMyCar.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace FixMyCar.Web.Controllers;

[Authorize]
public class PostController : Controller
{
    private readonly IPostService _postService;
    private readonly ICategoryService _categoryService;
    private readonly IUserService _userService;

    public PostController(
        IPostService postService,
        ICategoryService categoryService,
        IUserService userService)
    {
        _postService = postService;
        _categoryService = categoryService;
        _userService = userService;
    }

    // GET: /Post
    public async Task<IActionResult> Index()
    {
        var posts = await _postService.GetAllAsync();
        return View(posts);
    }

    // GET: /Post/Create
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var categories = await _categoryService.GetAllAsync();

        var vm = new PostCreateViewModel
        {
            Categories = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList()
        };

        return View(vm);
    }

    // POST: /Post/Create
    [HttpPost]
    public async Task<IActionResult> Create(PostCreateViewModel vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
            return RedirectToAction("Login", "User");

        var post = new Post
        {
            Title = vm.Title,
            Description = vm.Description,
            City = vm.City,
            CategoryId = vm.CategoryId,

            UserId = int.Parse(userId)   // ✅ FIX HERE
        };

        await _postService.CreateAsync(post);
        if (!int.TryParse(userId, out var parsedUserId))
        {
            return RedirectToAction("Login", "User");
        }
        return RedirectToAction("Index");
    }

    // GET: /Post/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var post = await _postService.GetByIdAsync(id);

        if (post == null)
            return NotFound();

        return View(post);
    }

    // GET: /Post/Delete/5
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var post = await _postService.GetByIdAsync(id);

        if (post == null)
            return NotFound();

        return View(post);
    }

    // POST: /Post/Delete/5
    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _postService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}