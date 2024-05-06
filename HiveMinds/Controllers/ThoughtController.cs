using System.Security.Claims;
using AutoMapper;
using HiveMinds.Adapters.Interfaces;
using HiveMinds.Common;
using HiveMinds.DTO;
using HiveMinds.Interfaces;
using HiveMinds.ViewModels;
using HiveMinds.ViewModels.Pages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiveMinds.Controllers;

[Route("/[controller]/")]
public class ThoughtController : Controller
{
    private readonly ILogger<ThoughtController> _logger;
    private readonly IModelToViewModelAdapter _modelToView;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;
    private readonly IThoughtService _thoughtService;

    public ThoughtController(ILogger<ThoughtController> logger, IModelToViewModelAdapter modelToView,
        IUserService userService, IThoughtService thoughtService, IMapper mapper)
    {
        _logger = logger;
        _modelToView = modelToView;
        _userService = userService;
        _thoughtService = thoughtService;
        _mapper = mapper;
    }

    [HttpGet("/[controller]")]
    [AllowAnonymous]
    public IActionResult Index()
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpGet("/[controller]/{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> Index(int id)
    {
        var thought = await _thoughtService.GetThought(id);
        if (thought == null) return RedirectToAction("Index", "Home");
        var user = await GetCurrentUser();
        if (user == null) return RedirectToAction("Index", "Home");


        var vm = new ThoughtPageViewModel()
        {
            CurrentUser = await _modelToView.GetUserViewModel(user),
            Thought = _mapper.Map<ThoughtViewModel>(thought)
        };
        return View(vm);
    }

    [HttpPost("/[action]")]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Post(string content)
    {
        if (User.Identity is { IsAuthenticated: false }) return Challenge();

        await _thoughtService.CreateThought(content);
        return Redirect(Request.Headers["Referer"].ToString());
    }

    [HttpPost("/[action]")]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reply(int id, string content)
    {
        if (User.Identity is { IsAuthenticated: false } or null) return Challenge();
        
        var user = await GetCurrentUser();
        if (user == null) return RedirectToPage("/Login");

        var thought = await _thoughtService.GetThought(id);

        if (thought == null || thought.User.Status != AccountStatus.Active)
            return Redirect(Request.Headers["Referer"].ToString());

        
        await _thoughtService.ReplyToThought(id, user.Username, content);

        return Redirect(Request.Headers["Referer"].ToString());
    }

    [HttpPost("/[action]")]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Like(int id)
    {
        if (User.Identity is { IsAuthenticated: false } or null) return Challenge();

        var thought = await _thoughtService.GetThought(id);
        if (thought == null) return RedirectToAction("Index", "Home");

        if (thought.User.Status != AccountStatus.Active) return Redirect(Request.Headers["Referer"].ToString());

        if (User.Identity.Name != null) await _thoughtService.LikeThought(id);
        return Redirect(Request.Headers["Referer"].ToString());
    }


    [HttpPost("/[action]")]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Unlike(int id)
    {
        if (User.Identity is { IsAuthenticated: false } or null) return Challenge();

        var thought = await _thoughtService.GetThought(id);
        if (thought == null) return RedirectToAction("Index", "Home");

        if (User.Identity.Name != null) await _thoughtService.UnlikeThought(id);

        if (thought.User.Status != AccountStatus.Active) return Redirect(Request.Headers["Referer"].ToString());

        return Redirect(Request.Headers["Referer"].ToString());
    }

    [HttpPost("/[action]")]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Report(int id)
    {
        if (User.Identity is { IsAuthenticated: false } or null) return Challenge();

        var thought = await _thoughtService.GetThought(id);
        if (thought == null) return RedirectToAction("Index", "Home");
        //if (User.Identity.Name != null) await _thoughtService.ReportThought(id, User.Identity.Name);

        if (thought.User.Status != AccountStatus.Active) return Redirect(Request.Headers["Referer"].ToString());

        return Redirect(Request.Headers["Referer"].ToString());
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