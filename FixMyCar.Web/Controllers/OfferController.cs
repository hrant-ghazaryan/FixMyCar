using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FixMyCar.Web.Models;
using FixMyCar.Web.Services;
using FixMyCar.Web.ViewModels;

namespace FixMyCar.Web.Controllers;

[Authorize]
public class OfferController(IOfferService offerService, IPostService postService) : Controller
{
    private readonly IOfferService _offerService = offerService;
    private readonly IPostService _postService = postService;

    // 📌 All offers (admin/debug)
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Index()
    {
        var offers = await _offerService.GetAllAsync();
        return View(offers);
    }

    // 📌 Offers for specific post
    public async Task<IActionResult> ByPost(int postId)
    {
        var post = await _postService.GetByIdAsync(postId);
        if (post == null)
            return NotFound();

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (post.UserId != userId && !User.IsInRole("Admin"))
            return Forbid();

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
    public async Task<IActionResult> Create(int postId, OfferCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.PostId = postId;
            return View(model);
        }

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
            Price = model.Price,
            Message = model.Message?.Trim() ?? string.Empty
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
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var offer = await _offerService.GetByIdAsync(id);
        if (offer == null)
            return NotFound();

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (offer.UserId != userId && !User.IsInRole("Admin"))
            return Forbid();

        return View(offer);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        try
        {
            await _offerService.DeleteAsync(id, userId);
        }
        catch (Exception ex) when (ex is KeyNotFoundException or UnauthorizedAccessException or InvalidOperationException)
        {
            TempData["OfferError"] = ex.Message;
        }

        return RedirectToAction("MyOffers", "User");
    }

    public async Task<IActionResult> Details(int id)
    {
        var offer = await _offerService.GetByIdAsync(id);

        if (offer == null)
            return NotFound();

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (offer.UserId != userId && !User.IsInRole("Admin"))
            return Forbid();

        return View(new OfferEditViewModel
        {
            Id = offer.Id,
            Price = offer.Price,
            Message = offer.Message
        });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var offer = await _offerService.GetByIdAsync(id);

        if (offer == null)
            return NotFound();

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (offer.UserId != userId && offer.Post.UserId != userId && !User.IsInRole("Admin"))
            return Forbid();

        return View(offer);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(OfferEditViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        try
        {
            await _offerService.UpdateAsync(model.Id, userId, model.Price, model.Message);
        }
        catch (Exception ex) when (ex is KeyNotFoundException or UnauthorizedAccessException or InvalidOperationException or ArgumentOutOfRangeException)
        {
            TempData["OfferError"] = ex.Message;
            return RedirectToAction("MyOffers", "User");
        }

        return RedirectToAction("MyOffers", "User");
    }
}
