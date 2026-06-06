using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FixMyCar.Web.Models;
using FixMyCar.Web.Services;

namespace FixMyCar.Web.Controllers;

[Authorize]
public class OfferController : Controller
{
    private readonly IOfferService _offerService;

    public OfferController(IOfferService offerService)
        => _offerService = offerService;

    // 📌 All offers (admin/debug)
    public async Task<IActionResult> Index()
    {
        var offers = await _offerService.GetAllAsync();
        return View(offers);
    }

    // 📌 Offers for specific post
    public async Task<IActionResult> ByPost(int postId)
    {
        var offers = await _offerService.GetByPostIdAsync(postId);
        ViewBag.PostId = postId;

        return View(offers);
    }

    // 📌 Create offer form
    [HttpGet]
    public IActionResult Create(int postId)
    {
        ViewBag.PostId = postId;
        return View();
    }

    // 📌 Save offer
    [HttpPost]
    public async Task<IActionResult> Create(int postId, decimal price, string? message)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var offer = new Offer
        {
            PostId = postId,
            UserId = int.Parse(userId!),
            Price = price,
            Message = message
        };

        await _offerService.CreateAsync(offer);

        return RedirectToAction("ByPost", new { postId });
    }

    // 📌 Delete offer
    public async Task<IActionResult> Delete(int id, int postId)
    {
        await _offerService.DeleteAsync(id);

        return RedirectToAction("ByPost", new { postId });
    }
    public async Task<IActionResult> Details(int id)
    {
        var offer = await _offerService.GetByIdAsync(id);

        if (offer == null)
            return NotFound();

        return View(offer);
    }
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var offer = await _offerService.GetByIdAsync(id);

        if (offer == null)
            return NotFound();

        return View(offer);
    }
    [HttpPost]
    public async Task<IActionResult> Edit(Offer offer)
    {
        if (!ModelState.IsValid)
            return View(offer);

        await _offerService.UpdateAsync(offer);

        return RedirectToAction("Index", "Profile");
    }
    public async Task<IActionResult> Delete(int id)
    {
        await _offerService.DeleteAsync(id);

        return RedirectToAction("Index", "Profile");
    }
}