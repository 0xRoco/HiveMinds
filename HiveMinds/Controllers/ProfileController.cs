using System.Security.Claims;
using HiveMinds.Adapters.Interfaces;
using HiveMinds.Services.Interfaces;
using HiveMinds.ViewModels;
using HiveMinds.ViewModels.Pages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiveMinds.Controllers;

[Route("[Controller]")]
public class ProfileController : Controller
{
    
    private readonly ILogger<ProfileController> _logger;
    private readonly IAuthService _auth;
    private readonly IModelToViewModelAdapter _modelToView;
    private readonly IUserService _userService;
    private readonly IThoughtService _thoughtService;

    public ProfileController(IAuthService auth,
        ILogger<ProfileController> logger, IModelToViewModelAdapter modelToView,
        IUserService userService, IThoughtService thoughtService)
    {
        _auth = auth;
        _logger = logger;
        _modelToView = modelToView;
        _userService = userService;
        _thoughtService = thoughtService;
    }


    [HttpGet]
    [Authorize]
    [Route("/profile/")]
    public async Task<IActionResult> Index()
    {
        if (User.Identity is { IsAuthenticated: false }) return Challenge();

        var user = await _userService.GetUser(User.FindFirstValue(ClaimTypes.Name) ?? string.Empty);
        if (user == null) return NotFound();
        var profile = await _modelToView.GetUserViewModel(user);

        var vm = new ProfilePageViewModel()
        {
            CurrentUser = profile,
            Profile = profile
        };
        
        return View(vm);
    }

    [HttpGet]
    [Authorize]
    [Route("/profile/edit")]
    public async Task<IActionResult> Edit()
    {
        if (User.Identity is { IsAuthenticated: false }) return Challenge();

        var user = await _userService.GetUser(User.Identity?.Name!);
        if (user == null) return NotFound();
        var profile = await _modelToView.GetUserViewModel(user);
        return View(profile);
    }

    [HttpPost]
    [Authorize]
    [Route("/profile/edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UserViewModel profile)
    {
        if (User.Identity is { IsAuthenticated: false }) return Challenge();
        var user = await _userService.GetUser(User.Identity?.Name!);
        if (user == null) return NotFound();
        
        user.Bio = profile.Bio;
        user.LoyaltyStatement = profile.PartyLoyaltyStatement;

        var result = false; //await _accountRepo.UpdateUser(user);
        
        if (!result)
        {
            return RedirectToAction("Index");
        }

        return View(profile);

    }

    [HttpGet]
    [Route("/profile/{username}")]
    [AllowAnonymous]
    public async Task<IActionResult> Index(string username)
    {
        var user = await _userService.GetUser(username);
        if (user == null) return NotFound();
        var profile = await _modelToView.GetUserViewModel(user);

        var localUser = await _userService.GetUser(User.FindFirstValue(ClaimTypes.Name) ?? string.Empty);
        if (localUser == null) return NotFound();
        var vm = new ProfilePageViewModel()
        {
            CurrentUser = await _modelToView.GetUserViewModel(localUser),
            Profile = profile
        };
        
        return View(vm);
    }

    [HttpGet]
    [Route("/profile/verify-email")]
    [Authorize]
    public async Task<IActionResult> VerifyEmail()
    {
        if (User.Identity is { IsAuthenticated: false }) return Challenge();
        var user = await _userService.GetUser(User.Identity?.Name!);
        if (user == null) return NotFound();
        
        return View();
    }

    [HttpPost]
    [Route("/profile/verify-email")]
    [Authorize]
    public async Task<IActionResult> VerifyEmail(string verificationCode)
    {
        if (User.Identity is { IsAuthenticated: false }) return Challenge();
        var user = await _userService.GetUser(User.Identity?.Name!);
        if (user == null) return NotFound();

        var result = false; //await _verification.VerifyEmail(user.Username, verificationCode);
        if (!result) return View();
        return RedirectToAction("Index");
    }

    [HttpGet]
    [Route("/profile/verify")]
    [Authorize]
    public async Task<IActionResult> Verify()
    {
        if (User.Identity is { IsAuthenticated: false }) return Challenge();
        var user = await _userService.GetUser(User.Identity?.Name!);
        if (user == null) return NotFound();
        
        if (user.IsVerified) return RedirectToAction("Index");
        
        return View();
    }
    
    [HttpPost]
    [Route("/profile/verify")]
    [Authorize]
    public async Task<IActionResult> Verify(string reason)
    {
        if (User.Identity is { IsAuthenticated: false }) return Challenge();
        var user = await _userService.GetUser(User.Identity?.Name!);
        if (user == null) return NotFound();
        
        if (user.IsVerified) return RedirectToAction("Index");

        var result = false; //await _verification.RequestVerification(user.Username, reason);
        if (!result) return View();
        return RedirectToAction("Index");
    }
    
}