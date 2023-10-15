using System.Security.Claims;
using HiveMinds.Models;
using HiveMinds.Services.Interfaces;
using HiveMinds.ViewModels;
using HiveMinds.ViewModels.Pages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiveMinds.Controllers;

[Route("/[controller]/")]
public class ThoughtController : Controller
{
    private readonly ILogger<ThoughtController> _logger;
    private readonly IThoughtService _thoughtService;
    private readonly IAccountRepository _accountRepo;
    private readonly IUtility _utility;

    public ThoughtController(ILogger<ThoughtController> logger, IThoughtService thoughtService,
        IAccountRepository accountRepo, IUtility utility)
    {
        _logger = logger;
        _thoughtService = thoughtService;
        _accountRepo = accountRepo;
        _utility = utility;
    }

    [HttpGet]
    [Route("/[controller]/")]
    [AllowAnonymous]
    public IActionResult Index()
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [Route("/[controller]/{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> Index(int id)
    {
        var thought = await _thoughtService.GetThought(id);
        if (thought == null) return RedirectToAction("Index", "Home");

        var user = _accountRepo.GetByUsername(thought.Username);
        if (user == null) return RedirectToAction("Index", "Home");


        var vm = new ThoughtPageViewModel()
        {
            CurrentUser = await _utility.GetUserViewModel(user),
            Thought = thought
        };
        return View(vm);
    }

    [HttpPost]
    [Route("[Action]")]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Post(string content)
    {
        if (User.Identity is { IsAuthenticated: false }) return RedirectToPage("/Login");
        var account = _accountRepo.GetByUsername(User.FindFirstValue(ClaimTypes.Name) ?? string.Empty);
        if (account == null) return RedirectToPage("/Login");
        await _thoughtService.CreateThought(account.Username, content);
        return Redirect(Request.Headers["Referer"].ToString());
    }

    [HttpPost("[Action]")]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reply(int id, string content)
    {
        if (User.Identity is { IsAuthenticated: false } or null) return RedirectToPage("/Login");
        var user = _accountRepo.GetByUsername(User.Identity?.Name!);
        if (user == null) return RedirectToPage("/Login");
        await _thoughtService.ReplyToThought(id, user.Username, content);

        return Redirect(Request.Headers["Referer"].ToString());
    }

    [HttpPost("[Action]")]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Like(int id)
    {
        if (User.Identity is { IsAuthenticated: false } or null) return RedirectToPage("/Login");

        var thought = await _thoughtService.GetThought(id);
        if (thought == null) return RedirectToAction("Index", "Home");
        if (User.Identity.Name != null) await _thoughtService.LikeThought(id, User.Identity.Name);
        _logger.LogInformation(Request.Path.ToString());
        return Redirect(Request.Headers["Referer"].ToString());
    }


    [HttpPost("[Action]")]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Unlike(int id)
    {
        if (User.Identity is { IsAuthenticated: false } or null) return RedirectToPage("/Login");

        var thought = await _thoughtService.GetThought(id);
        if (thought == null) return RedirectToAction("Index", "Home");
        if (User.Identity.Name != null) await _thoughtService.UnlikeThought(id, User.Identity.Name);
        _logger.LogInformation(Request.Path.ToString());

        return Redirect(Request.Headers["Referer"].ToString());
    }
    
    [HttpPost("[Action]")]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Report(int id)
    {
        if (User.Identity is { IsAuthenticated: false } or null) return RedirectToPage("/Login");

        var thought = await _thoughtService.GetThought(id);
        if (thought == null) return RedirectToAction("Index", "Home");
        //if (User.Identity.Name != null) await _thoughtService.ReportThought(id, User.Identity.Name);
        _logger.LogInformation(Request.Path.ToString());

        return Redirect(Request.Headers["Referer"].ToString());
    }
    
}