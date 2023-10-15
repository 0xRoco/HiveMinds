using System.Security.Claims;
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
    private readonly IAccountRepository _account;
    private readonly ILogger<AuthController> _logger;
    private readonly IAuthService _auth;
    public AuthController(IAccountRepository account, ILogger<AuthController> logger, IAuthService auth)
    {
        _account = account;
        _logger = logger;
        _auth = auth;
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
        if (!ModelState.IsValid)
        {
            _logger.LogInformation("Login attempt failed");
            ModelState.AddModelError("Login", "Login attempt failed");
            return View(loginModel);
        }
        
        var result = await _auth.Login(loginModel);
        
        if (result == false)
        {
            _logger.LogInformation($"Login attempt for {loginModel.Username} failed");
            ModelState.AddModelError("Login", "Login attempt failed");
            return View(new LoginViewModel());
        }
        
        var user = _account.GetByUsername(loginModel.Username);
        if (user == null)
        {
            _logger.LogInformation($"User {loginModel.Username} not found");
            ModelState.AddModelError("Login", "Login attempt failed");
            return View(loginModel);
        }
        
        var claims = new List<Claim>
        {
            new (ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, loginModel.Username),
            new(ClaimTypes.Role, user.IsAdmin ? "Admin" : "User"),
            new("IsVerified", user.IsVerified.ToString()),
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(claimsIdentity);
        

        await HttpContext.SignInAsync(principal, new AuthenticationProperties
        {
            IsPersistent = loginModel.RememberMe,
            ExpiresUtc = DateTime.UtcNow.AddYears(1),
        });

        _logger.LogInformation($"User {loginModel.Username} logged in - RememberMe {loginModel.RememberMe}");
        return RedirectToAction("Index", "Home");
    }
    
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Signup(SignupViewModel signupModel)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogInformation("Signup attempt failed");
            ModelState.AddModelError("Signup", "Signup attempt failed - Invalid model");
            return View(signupModel);
        }
        
        if (signupModel.Password != signupModel.ConfirmPassword)
        {
            ModelState.AddModelError("Signup", "Signup attempt failed - Passwords do not match");
            return View(signupModel);
        }
        
        var result = await _auth.Signup(signupModel);
        
        if (result == false)
        {
            _logger.LogInformation($"Signup attempt for {signupModel.Username} failed");
            ModelState.AddModelError("Signup", "Signup attempt failed");
            return View(new SignupViewModel());
        }
        
        _logger.LogInformation($"User {signupModel.Username} signed up");

        return await Login(new LoginViewModel()
        {
            Username = signupModel.Username,
            Password = signupModel.Password
        });
    }
}