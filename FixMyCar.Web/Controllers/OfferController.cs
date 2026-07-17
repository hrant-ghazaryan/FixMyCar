using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FixMyCar.Web.Models;
using FixMyCar.Web.Services;

namespace FixMyCar.Web.Controllers;

[Authorize]
public class OfferController(IOfferService offerService, IPostService postService) : Controller
{
    private readonly IOfferService _offerService = offerService;
    private readonly IPostService _postService = postService;

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

    // 📌 Create offer form (GET)
    [HttpGet]
    public async Task<IActionResult> Create(int postId)
    {
        var post = await _postService.GetByIdAsync(postId);

        if (post == null)
            return NotFound();

        // Cannot create offer on closed post
        if (post.ClosedAt != null)
        {
            TempData["OfferError"] = "This post is already closed and not accepting new offers.";
            return RedirectToAction("Details", "Post", new { id = postId });
        }

        // Post owner cannot make an offer on their own post
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (post.UserId == userId)
        {
            TempData["OfferError"] = "You cannot make an offer on your own post.";
            return RedirectToAction("Details", "Post", new { id = postId });
        }

        ViewBag.PostId = postId;
        return View();
    }

    // 📌 Save offer (POST)
    [HttpPost]
    public async Task<IActionResult> Create(int postId, decimal price, string? message)
    {
        var post = await _postService.GetByIdAsync(postId);

        if (post == null)
            return NotFound();

        if (post.ClosedAt != null)
        {
            TempData["OfferError"] = "This post is already closed.";
            return RedirectToAction("Details", "Post", new { id = postId });
        }

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        if (post.UserId == userId)
        {
            TempData["OfferError"] = "You cannot make an offer on your own post.";
            return RedirectToAction("Details", "Post", new { id = postId });
        }

        var offer = new Offer
        {
            PostId = postId,
            UserId = userId,
            Price = price,
            Message = message ?? string.Empty
        };

        await _offerService.CreateAsync(offer);

        return RedirectToAction("Details", "Post", new { id = postId });
    }

    // 📌 Accept offer (POST)
    [HttpPost]
    public async Task<IActionResult> Accept(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        try
        {
            var offer = await _offerService.GetByIdAsync(id);
            int postId = offer?.PostId ?? 0;

            await _offerService.AcceptOfferAsync(id, userId);

            TempData["OfferSuccess"] = "Offer accepted! The post has been closed.";
            return RedirectToAction("Details", "Post", new { id = postId });
        }
        catch (Exception ex)
        {
            TempData["OfferError"] = ex.Message;
            return RedirectToAction("Index", "Home");
        }
    }

    // 📌 Decline offer (POST)
    [HttpPost]
    public async Task<IActionResult> Decline(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        try
        {
            var offer = await _offerService.GetByIdAsync(id);
            int postId = offer?.PostId ?? 0;

            await _offerService.DeclineOfferAsync(id, userId);

            TempData["OfferSuccess"] = "Offer declined.";
            return RedirectToAction("Details", "Post", new { id = postId });
        }
        catch (Exception ex)
        {
            TempData["OfferError"] = ex.Message;
            return RedirectToAction("Index", "Home");
        }
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
}