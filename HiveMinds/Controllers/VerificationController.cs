using HiveMinds.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sentry;

namespace HiveMinds.Controllers;

public class VerificationController : Controller
{
    private readonly IUserService _userService;

    public VerificationController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Index()
    {
        if (User.Identity is { IsAuthenticated: false }) return Challenge();
        var apiResponse = await _userService.GetUser(User.Identity?.Name ??
                                                     throw new InvalidOperationException("User is not authenticated"));
        if (apiResponse is { Success: false }) return NotFound();
        var user = apiResponse?.Data;

        if (user == null) return NotFound();

        if (user.IsVerified) return RedirectToAction("Index", "Home");

        return View();
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Index(string reason)
    {
        if (User.Identity is { IsAuthenticated: false }) return Challenge();
        var apiResponse = await _userService.GetUser(User.Identity?.Name ??
                                                     throw new InvalidOperationException("User is not authenticated"));
        if (apiResponse is { Success: false }) return NotFound();
        var user = apiResponse?.Data;

        if (user == null) return NotFound();

        if (user.IsVerified) return RedirectToAction("Index", "Home");

        var result = false; //await _verification.RequestVerification(user.Username, reason);
        if (!result) return View();
        return RedirectToAction("Index", "Home");
    }
}