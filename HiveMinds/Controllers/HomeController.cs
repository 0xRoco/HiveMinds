using System.Security.Claims;
using AutoMapper;
using HiveMinds.Adapters.Interfaces;
using HiveMinds.Interfaces;
using HiveMinds.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiveMinds.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IModelToViewModelAdapter _modelToView;
    private readonly IUserService _userService;
    private readonly IThoughtService _thoughtService;
    private readonly IMapper _mapper;

    public HomeController(ILogger<HomeController> logger, IModelToViewModelAdapter modelToView,
        IUserService userService, IThoughtService thoughtService, IMapper mapper)
    {
        _logger = logger;
        _modelToView = modelToView;
        _userService = userService;
        _thoughtService = thoughtService;
        _mapper = mapper;
    }
    
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Index()
    {
        if (User.Identity is { IsAuthenticated: false }) return Challenge();

        var apiResponse = await _userService.GetUser(User.FindFirstValue(ClaimTypes.Name) ?? string.Empty);
        if (apiResponse is { Success: false, Data: null }) return Challenge();
        var account = apiResponse?.Data;
        if (account == null)
        {
            await HttpContext.SignOutAsync();
            return Challenge();
        }
        var vm = new HomeViewModel
        {
            User = await _modelToView.GetUserViewModel(account),
            Thoughts = _mapper.Map<List<ThoughtViewModel>>(await _thoughtService.GetThoughts())
        };
        
        return View(vm);
    }
    
}