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
    // PROFILE (current logged-in user)
    // =========================
    public async Task<IActionResult> Profile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
            return RedirectToAction("Login");

        var user = await _userService.GetByIdAsync(int.Parse(userId));

        if (user == null)
            return NotFound();

        var userPosts = await _postService.GetByUserIdAsync(user.Id);
        ViewBag.UserPosts = userPosts;
        var offers = await _offerService.GetByUserIdAsync(user.Id);
        ViewBag.UserOffers = offers;

        return View(user);
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