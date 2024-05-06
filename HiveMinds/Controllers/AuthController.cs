using System.Security.Claims;
using HiveMinds.DTO;
using HiveMinds.Interfaces;
using HiveMinds.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiveMinds.Controllers;

[Route("[action]"), Authorize]
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
    public async Task<IActionResult> Logout()
    {
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
                ModelState.AddModelError("Login", "Login attempt failed - Invalid state");
                return View(loginModel);
            }

            var loginResponse = await _auth.Login(new LoginDto
            {
                Username = loginModel.Username,
                Password = loginModel.Password
            });

            if (loginResponse?.Data is null || loginResponse is { Success: false } ||
                string.IsNullOrEmpty(loginResponse.Data.Token))
            {
                ModelState.AddModelError("Login",
                    loginResponse?.Message ?? "Login attempt failed - Invalid credentials");
                return View(loginModel);
            }

            Response.Cookies.Append("token", loginResponse.Data.Token, new CookieOptions()
            {
                Expires = loginResponse.Data.Expiration,
                HttpOnly = true,
                Secure = true
            });

            var userResponse = await _userService.GetUser(loginModel.Username);
            if (userResponse is { Success: false }) return BadRequest(userResponse.Message);
            var user = userResponse?.Data;

            if (user == null)
            {
                ModelState.AddModelError("Login", "Login attempt failed - User not found");
                return View(loginModel);
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Username)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(claimsIdentity);


            await HttpContext.SignInAsync(principal, new AuthenticationProperties
            {
                IsPersistent = loginModel.RememberMe,
                ExpiresUtc = loginResponse.Data.Expiration
            });
            
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
                ModelState.AddModelError("Signup", "Signup attempt failed - Invalid state");
                return View(signupModel);
            }

            var model = new SignupDto()
            {
                Username = signupModel.Username,
                Password = signupModel.Password,
                Bio = signupModel.Bio,
                Email = signupModel.Email,
                PhoneNumber = signupModel.PhoneNumber,
            };

            var result = await _auth.Signup(model);

            if (result) return RedirectToAction("Login", "Auth");

            ModelState.AddModelError("Signup", "Signup attempt failed - Invalid credentials");
            return View(new SignupViewModel());

        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while signing up");
            return View(signupModel);
        }
    }
}