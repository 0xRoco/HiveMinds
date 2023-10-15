using System.Security.Claims;
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
    private readonly IVerificationService _verification;
    private readonly IAccountRepository _accountRepo;
    private readonly IThoughtService _thoughtRepo;
    private readonly IUtility _utility;

    public ProfileController(IThoughtService thoughtRepo, IAccountRepository accountRepo, IAuthService auth, ILogger<ProfileController> logger, IUtility utility, IVerificationService verification)
    {
        _thoughtRepo = thoughtRepo;
        _accountRepo = accountRepo;
        _auth = auth;
        _logger = logger;
        _utility = utility;
        _verification = verification;
    }


    [HttpGet]
    [Authorize]
    [Route("/profile/")]
    public async Task<IActionResult> Index()
    {
        if (User.Identity is { IsAuthenticated: false }) return Challenge();
        
        var user = _accountRepo.GetByUsername(User.FindFirstValue(ClaimTypes.Name) ?? string.Empty);
        if (user == null) return NotFound();
        var profile = await _utility.GetUserViewModel(user);

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
        
        var user = _accountRepo.GetByUsername(User.Identity?.Name!);
        if (user == null) return NotFound();
        var profile = await _utility.GetUserViewModel(user);
        return View(profile);
    }

    [HttpPost]
    [Authorize]
    [Route("/profile/edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UserViewModel profile)
    {
        if (User.Identity is { IsAuthenticated: false }) return Challenge();
        var user = _accountRepo.GetByUsername(User.Identity?.Name!);
        if (user == null) return NotFound();
        
        user.Bio = profile.Bio;
        user.LoyaltyStatement = profile.PartyLoyaltyStatement;
        
        var result = await _accountRepo.UpdateUser(user);
        
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
        var user = _accountRepo.GetByUsername(username);
        if (user == null) return NotFound();
        var profile = await _utility.GetUserViewModel(user);
        
        var vm = new ProfilePageViewModel()
        {
            CurrentUser = await GetUserViewModel(User.Identity?.Name!),
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
        var user = _accountRepo.GetByUsername(User.Identity?.Name!);
        if (user == null) return NotFound();
        
        if (user.IsEmailVerified) return RedirectToAction("Index");
        return View();
    }

    [HttpPost]
    [Route("/profile/verify-email")]
    [Authorize]
    public async Task<IActionResult> VerifyEmail(string verificationCode)
    {
        if (User.Identity is { IsAuthenticated: false }) return Challenge();
        var user = _accountRepo.GetByUsername(User.Identity?.Name!);
        if (user == null) return NotFound();
        
        if (user.IsEmailVerified) return RedirectToAction("Index");
        
        var result = await _verification.VerifyEmail(user.Username, verificationCode);
        if (!result) return View();
        return RedirectToAction("Index");
    }

    [HttpGet]
    [Route("/profile/verify")]
    [Authorize]
    public async Task<IActionResult> Verify()
    {
        if (User.Identity is { IsAuthenticated: false }) return Challenge();
        var user = _accountRepo.GetByUsername(User.Identity?.Name!);
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
        var user = _accountRepo.GetByUsername(User.Identity?.Name!);
        if (user == null) return NotFound();
        
        if (user.IsVerified) return RedirectToAction("Index");

        var result = await _verification.RequestVerification(user.Username, reason);
        if (!result) return View();
        return RedirectToAction("Index");
    }

    private async Task<UserViewModel> GetUserViewModel(string username)
    {
        var user = _accountRepo.GetByUsername(username);
        if (user == null) return new UserViewModel();
        
        var profile = await _utility.GetUserViewModel(user);
        return profile;
    }
    
}