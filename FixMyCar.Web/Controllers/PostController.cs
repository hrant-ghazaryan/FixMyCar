using FixMyCar.Web.Models;
using FixMyCar.Web.Services;
using FixMyCar.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace FixMyCar.Web.Controllers;

public class PostController(IPostService postService, ICategoryService categoryService,
                      IUserService userService, IOfferService offerService) : Controller
{
    private readonly IPostService _postService = postService;
    private readonly ICategoryService _categoryService = categoryService;
    private readonly IUserService _userService = userService;
    private readonly IOfferService _offerService = offerService;

    // GET: /Post
    public async Task<IActionResult> Index()
    {
        var posts = await _postService.GetAllAsync();
        return View(posts);
    }

    // GET: /Post/Create
    [Authorize(Roles ="User,Admin")]
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
    [Authorize(Roles = "User")]
    [HttpPost]
    public async Task<IActionResult> Create(PostCreateViewModel vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var post = new Post
        {
            Title = vm.Title,
            Description = vm.Description,
            City = vm.City,
            CategoryId = vm.CategoryId,
            UserId = userId,
            Media = new List<PostMedia>()
        };

        await _postService.CreateAsync(post);

        // ⭐ HERE STARTS IMAGE UPLOAD
        if (vm.Files != null && vm.Files.Count > 0)
        {
            var uploadsFolder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot/uploads"
            );

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            int order = 0;

            foreach (var file in vm.Files)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);

                var fullPath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                post.Media.Add(new PostMedia
                {
                    FilePath = "/uploads/" + fileName,
                    Type = MediaType.Image,
                    Order = order,
                    IsMain = order == 0,
                    CreatedAt = DateTime.UtcNow,
                    PostId = post.Id
                });

                order++;
            }

            await _postService.UpdateAsync(post);
        }

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> ByCategory(int categoryId)
    {
        var posts = await _postService.GetAllAsync();

        var result = posts
            .Where(x => x.CategoryId == categoryId)
            .ToList();

        return View("Index", result);
    }

    // GET: /Post/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var post = await _postService.GetByIdAsync(id);

        if (post == null)
            return NotFound();

        var offers = await _offerService.GetByPostIdAsync(id);

        ViewBag.Offers = offers;
        await _postService.IncrementViewCountAsync(id);

        return View(post);
    }

    // GET: /Post/Delete/5
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var post = await _postService.GetByIdAsync(id);

        if (post == null)
            return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (post.UserId != int.Parse(userId!))
            return Forbid();

        return View(post);
    }


    // POST: /Post/Delete/5
    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var post = await _postService.GetByIdAsync(id);

        if (post == null)
            return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (post.UserId != int.Parse(userId!))
            return Forbid();

        await _postService.DeleteAsync(id);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var post = await _postService.GetByIdAsync(id);

        if (post == null)
            return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (post.UserId != int.Parse(userId!))
            return Forbid();

        ViewBag.Categories = await _categoryService.GetAllAsync();

        return View(post);
    }
    [HttpPost]
    public async Task<IActionResult> Edit(Post model)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (model.UserId != int.Parse(userId!))
            return Forbid();

        await _postService.UpdateAsync(model);

        return RedirectToAction("Profile", "User");
    }

}