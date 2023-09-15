using AutoMapper;
using HiveMinds.API.Interfaces;
using HiveMinds.Common;
using HiveMinds.Models;
using HiveMinds.Services.Interfaces;
using HiveMinds.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace HiveMinds.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IAccountRepository _accountRepo;
    private readonly IThoughtService _thoughtService;
    private readonly IUtility _utility;
    private readonly IOptions<HiveMindsSettings> _settings;
    private readonly HomeViewModel _vm = new();

    public HomeController(ILogger<HomeController> logger, IThoughtService thoughtService, IAccountRepository accountRepo, IUtility utility, IOptions<HiveMindsSettings> settings)
    {
        _logger = logger;
        _thoughtService = thoughtService;
        _accountRepo = accountRepo;
        _utility = utility;
        _settings = settings;
    }
    
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Index()
    {
        if (User.Identity is { IsAuthenticated: false }) return RedirectToPage("/Login");
        var account =  _accountRepo.GetByUsername(User.Identity?.Name!);
        if (account == null)
        {
            await HttpContext.SignOutAsync();
            return View();
        }
        _vm.User = await _utility.GetUserViewModel(account);
        _vm.Thoughts = await _thoughtService.GetThoughts();
        
        _logger.LogInformation($"{_settings.Value.DefaultProfilePicture} | {_settings.Value.EmailConfig.Username}");
        return View(_vm);
    }
    
}