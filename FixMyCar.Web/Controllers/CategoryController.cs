using FixMyCar.Web.Models;
using FixMyCar.Web.Services;
using FixMyCar.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FixMyCar.Web.Controllers;

public class CategoryController(ICategoryService service, IPostService postservice) : Controller
{
    private readonly ICategoryService _service = service;
    private readonly IPostService _postservice = postservice;

    // GET: /Category
    public async Task<IActionResult> Index()
    {
        var categories = await _service.GetAllAsync();

        var parents = categories
            .Where(x => x.ParentId == null)
            .ToList();

        return View(parents);
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var categories = await _service.GetAllAsync();

        var vm = new CategoryCreateViewModel
        {
            ParentCategories = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList()
        };

        return View(vm);
    }
    public async Task<IActionResult> ByCategory(int categoryId)
    {
        var posts = await _postservice.GetAllAsync();

        var filtered = posts
            .Where(p => p.CategoryId == categoryId)
            .ToList();

        return View("Index", filtered);
    }
    public async Task<IActionResult> SubCategories(int parentId)
    {
        var categories = await _service.GetAllAsync();

        var children = categories
            .Where(c => c.ParentId == parentId)
            .ToList();

        return View(children);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CategoryCreateViewModel vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var category = new Category
        {
            Name = vm.Name,
            ParentId = vm.ParentId
        };

        await _service.AddAsync(category);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var category = await _service.GetByIdAsync(id);

        if (category == null)
            return NotFound();

        return View(category);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Category model)
    {
        if (!ModelState.IsValid)
            return View(model);

        await _service.UpdateAsync(model);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _service.GetByIdAsync(id);

        if (category == null)
            return NotFound();

        return View(category);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _service.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }

    /*public async Task<IActionResult> GetSubCategories(int parentId)
    {
        var categories = await _service.GetByParentIdAsync(parentId);

        return Json(categories.Select(c => new
        {
            id = c.Id,
            name = c.Name
        }));
    }*/
}