using AutoMapper;
using HiveMinds.DTO;
using HiveMinds.Interfaces;
using HiveMinds.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiveMinds.Controllers;

public class VerificationController : Controller
{
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public VerificationController(IUserService userService, IMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
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

        var alreadyRequested = await _userService.GetVerificationRequest(user.Id);

        var vm = new VerificationViewModel()
        {
            User = _mapper.Map<UserViewModel>(user),
            VerificationRequest = alreadyRequested?.Data
        };

        return View(vm);
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

        var alreadyRequested = await _userService.GetVerificationRequest(user.Id);

        if (alreadyRequested is { Success: true }) return RedirectToAction("Index", "Home");

        var requestResponse = await _userService.CreateVerificationRequest(new VerificationRequestDto
        {
            AccountId = user.Id,
            Reason = reason
        });

        if (requestResponse is { Success: false }) return View();
        
        return RedirectToAction("Index", "Home");
    }
}