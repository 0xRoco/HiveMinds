using System.Security.Claims;
using HiveMinds.Adapters.Interfaces;
using HiveMinds.DTO;
using HiveMinds.Interfaces;
using HiveMinds.ViewModels;
using HiveMinds.ViewModels.Pages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiveMinds.Controllers;

[Route("[Controller]")]
public class ProfileController : Controller
{
    
    private readonly ILogger<ProfileController> _logger;
    private readonly IModelToViewModelAdapter _modelToView;
    private readonly IUserService _userService;

    public ProfileController(ILogger<ProfileController> logger, IModelToViewModelAdapter modelToView,
        IUserService userService)
    {
        _logger = logger;
        _modelToView = modelToView;
        _userService = userService;
    }


    [HttpGet]
    [Authorize]
    [Route("/profile/")]
    public async Task<IActionResult> Index()
    {
        if (User.Identity is { IsAuthenticated: false }) return Challenge();

        var user = await GetCurrentUser();
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

        var user = await GetCurrentUser();
        if (user == null) return NotFound();
        var profile = await _modelToView.GetUserViewModel(user);

        var vm = new EditProfileViewModel
        {
            Bio = profile.Bio,
        };

        return View(vm);
    }

    [HttpPost]
    [Authorize]
    [Route("/profile/edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditProfileViewModel profile)
    {
        if (User.Identity is { IsAuthenticated: false }) return Challenge();
        var user = await GetCurrentUser();
        if (user == null) return NotFound();

        var editResponse = await _userService.UpdateUserProfile(user.Username, new EditProfileDto()
        {
            Bio = profile.Bio,
        }, profile.ProfilePicture);

        return RedirectToAction("Edit");
    }

    [HttpGet]
    [Route("/profile/{username}")]
    [AllowAnonymous]
    public async Task<IActionResult> Index(string username)
    {
        var apiResponse = await _userService.GetUser(username);
        if (apiResponse is { Success: false, Data: null }) return NotFound();
        var user = apiResponse?.Data;
        if (user == null) return NotFound();
        var profile = await _modelToView.GetUserViewModel(user);

        var localUser = await GetCurrentUser();
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
        var user = await GetCurrentUser();
        if (user == null) return NotFound();
        
        return View();
    }

    [HttpPost]
    [Route("/profile/verify-email")]
    [Authorize]
    public async Task<IActionResult> VerifyEmail(string verificationCode)
    {
        if (User.Identity is { IsAuthenticated: false }) return Challenge();
        var user = await GetCurrentUser();
        if (user == null) return NotFound();

        var apiResponse = await _userService.VerifyEmail(new VerifyEmailDto()
        {
            Username = user.Username,
            VerificationCode = verificationCode
        });

        if (apiResponse is { Success: false, Data: null }) return View();
        
        return RedirectToAction("Index");
    }

    // TODO: Move this to a service
    private async Task<UserDto?> GetCurrentUser()
    {
        var username = User.FindFirstValue(ClaimTypes.Name);
        if (username == null) return null;
        var apiResponse = await _userService.GetUser(username);
        if (apiResponse is { Success: false, Data: null }) return null;
        var user = apiResponse?.Data;
        return user;
    }
    
}