using System.Security.Claims;
using HiveMinds.DTO;
using HiveMinds.Services.Interfaces;
using HiveMinds.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiveMinds.Controllers;

[Route("[action]")]
public class AuthController : Controller
{
    private readonly ILogger<AuthController> _logger;
    private readonly IAuthService _auth;
    private readonly IUserService _userService;

    public AuthController(ILogger<AuthController> logger, IAuthService auth, IUserService userService)
    {
        _logger = logger;
        _auth = auth;
        _userService = userService;
    }
    
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login()
    {
        if (User.Identity is { IsAuthenticated: true }) return RedirectToAction("Index", "Home");
        return View();
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        _logger.LogInformation($"User {User.Identity?.Name} logged out");
        Response.Cookies.Delete("token");
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }
    
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Signup()
    {
        if (User.Identity is { IsAuthenticated: true }) return RedirectToAction("Index", "Home");
        return View();
    }
    
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel loginModel)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogDebug("Login attempt failed");
                ModelState.AddModelError("Login", "Login attempt failed");
                return View(loginModel);
            }

            var response = await _auth.Login(new LoginDto
            {
                Username = loginModel.Username,
                Password = loginModel.Password
            });

            if (string.IsNullOrEmpty(response.Token))
            {
                _logger.LogDebug("Login attempt failed for {Username} - Invalid credentials", loginModel.Username);
                ModelState.AddModelError("Login", "Login attempt failed");
                return View(new LoginViewModel());
            }

            Response.Cookies.Append("token", response.Token, new CookieOptions()
            {
                Expires = response.Expiration,
                HttpOnly = true,
            });

            var user = await _userService.GetUser(loginModel.Username);

            if (user == null)
            {
                _logger.LogDebug($"User {loginModel.Username} not found");
                ModelState.AddModelError("Login", "Login attempt failed");
                return View(loginModel);
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Username),
                // new(ClaimTypes.Role, user.IsAdmin ? "Admin" : "User"),
                new("IsVerified", user.IsVerified.ToString()),
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(claimsIdentity);


            await HttpContext.SignInAsync(principal, new AuthenticationProperties
            {
                IsPersistent = loginModel.RememberMe,
                ExpiresUtc = response.Expiration,
            });

            _logger.LogDebug($"User {loginModel.Username} logged in - RememberMe {loginModel.RememberMe}");
            return RedirectToAction("Index", "Home");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while logging in");
            return View(loginModel);
        }
    }
    
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Signup(SignupViewModel signupModel)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogDebug("Signup attempt failed");
                ModelState.AddModelError("Signup", "Signup attempt failed - Invalid model");
                return View(signupModel);
            }

            var model = new SignupDto()
            {
                Username = signupModel.Username,
                Password = signupModel.Password,
                PartyLoyaltyStatement = signupModel.PartyLoyaltyStatement,
                Bio = signupModel.Bio,
                Email = signupModel.Email,
                PhoneNumber = signupModel.PhoneNumber,
            };

            var result = await _auth.Signup(model);

            if (result == false)
            {
                _logger.LogDebug($"Signup attempt for {signupModel.Username} failed");
                ModelState.AddModelError("Signup", "Signup attempt failed");
                return View(new SignupViewModel());
            }

            _logger.LogDebug($"User {signupModel.Username} signed up");

            return View("Login");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while signing up");
            return View(signupModel);
        }
    }
}