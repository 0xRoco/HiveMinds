using System.Security.Claims;
using AutoMapper;
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
        if (User.Identity is { IsAuthenticated: false }) return Challenge();
        var account =  _accountRepo.GetByUsername(User.FindFirstValue(ClaimTypes.Name) ?? string.Empty);
        if (account == null)
        {
            await HttpContext.SignOutAsync();
            return Challenge();
        }
        var vm = new HomeViewModel
        {
            User = await _utility.GetUserViewModel(account),
            Thoughts = await _thoughtService.GetThoughts()
        };
        return View(vm);
    }
    
}