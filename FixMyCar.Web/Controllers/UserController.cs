using System.Security.Claims;
using FixMyCar.Web.Models;
using FixMyCar.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace FixMyCar.Web.Controllers;

public class UserController(IUserService userService, IPostService postService,
    IOfferService offerService) : Controller
{
    private readonly IUserService _userService = userService;
    private readonly IPostService _postService = postService;
    private readonly IOfferService _offerService = offerService;

    // =========================
    // REGISTER (GET)
    // =========================
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    // =========================
    // REGISTER (POST)
    // =========================
    [HttpPost]
    public async Task<IActionResult> Register(User model, string password)
    {
        if (!ModelState.IsValid)
            return View(model);

        model.PasswordHash = password;

        try
        {
            await _userService.RegisterAsync(model);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(model);
        }

        return RedirectToAction("Login");
    }

    // =========================
    // LOGIN (GET)
    // =========================
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    // =========================
    // LOGIN (POST)
    // =========================
    [HttpPost]
    public async Task<IActionResult> Login(string email, string password)
    {
        var user = await _userService.GetByEmailAsync(email);

        if (user == null)
        {
            ModelState.AddModelError("", "Invalid credentials");
            return View();
        }

        bool isValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

        if (!isValid)
        {
            ModelState.AddModelError("", "Invalid credentials");
            return View();
        }

        // 🔐 COOKIE AUTH
        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.Email, user.Email),

        // ⭐ ADD THIS LINE
        new Claim(ClaimTypes.Role, user.Role)   // <-- IMPORTANT
    };

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme
        );

        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal
        );

        return RedirectToAction("Index", "Home");
    }

    // =========================
    // LOGOUT
    // =========================
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }

    // =========================
    // PROFILE (current logged-in user or specified user)
    // =========================
    public async Task<IActionResult> Profile(int? id, [FromServices] IReviewService reviewService)
    {
        var loggedInUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        int targetUserId;
        bool isOwnProfile = false;

        if (id == null)
        {
            if (loggedInUserIdStr == null)
                return RedirectToAction("Login");

            targetUserId = int.Parse(loggedInUserIdStr);
            isOwnProfile = true;
        }
        else
        {
            targetUserId = id.Value;
            if (loggedInUserIdStr != null && int.Parse(loggedInUserIdStr) == targetUserId)
            {
                isOwnProfile = true;
            }
        }

        var user = await _userService.GetByIdAsync(targetUserId);
        if (user == null)
            return NotFound();

        var userPosts = await _postService.GetByUserIdAsync(user.Id);
        ViewBag.UserPosts = userPosts;
        var offers = await _offerService.GetByUserIdAsync(user.Id);
        ViewBag.UserOffers = offers;

        // Fetch ratings and reviews
        var reviews = await reviewService.GetReviewsForUserAsync(targetUserId);
        var averageRating = await reviewService.GetAverageRatingForUserAsync(targetUserId);

        ViewBag.Reviews = reviews;
        ViewBag.AverageRating = averageRating;
        ViewBag.IsOwnProfile = isOwnProfile;

        // Check if current user can review
        bool canLeaveReview = false;
        if (!isOwnProfile && loggedInUserIdStr != null)
        {
            int reviewerId = int.Parse(loggedInUserIdStr);
            var alreadyReviewed = await reviewService.HasUserReviewedAsync(reviewerId, targetUserId);
            canLeaveReview = !alreadyReviewed;
        }
        ViewBag.CanLeaveReview = canLeaveReview;

        return View(user);
    }

    // =========================
    // ADD REVIEW (POST)
    // =========================
    [HttpPost]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public async Task<IActionResult> AddReview(int targetUserId, int rating, string comment, [FromServices] IReviewService reviewService)
    {
        var loggedInUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (loggedInUserIdStr == null)
            return Challenge();

        int reviewerId = int.Parse(loggedInUserIdStr);

        var review = new Review
        {
            ReviewerId = reviewerId,
            TargetUserId = targetUserId,
            Rating = rating,
            Comment = comment ?? string.Empty,
            CreatedAt = DateTime.UtcNow
        };

        try
        {
            await reviewService.AddReviewAsync(review);
        }
        catch (Exception ex)
        {
            TempData["ReviewError"] = ex.Message;
        }

        return RedirectToAction("Profile", new { id = targetUserId });
    }

    public async Task<IActionResult> MyOffers()
    {
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!
        );

        var offers = await _offerService
            .GetByUserIdAsync(userId);

        return View(offers);
    }

    public async Task<IActionResult> Settings()
    {
        var userId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!
        );

        var userDetails = await _userService.GetByIdAsync(userId);

        if (userDetails == null)
            return NotFound();

        return View(userDetails);
    }
}