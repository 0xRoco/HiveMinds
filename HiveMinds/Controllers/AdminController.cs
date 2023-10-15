using System.Security.Claims;
using HiveMinds.Models;
using HiveMinds.Services.Interfaces;
using HiveMinds.ViewModels.Pages.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HiveMinds.Controllers;

public class AdminController : Controller
{
    private readonly IAccountRepository _accountRepository;
    private readonly IThoughtService _thoughtService;
    private readonly IThoughtRepository _thoughtRepository;
    private readonly IAdminService _adminService;
    private readonly IVerificationService _verificationService;

    public AdminController(IAccountRepository accountRepository, IThoughtService thoughtService, IVerificationService verificationService, IThoughtRepository thoughtRepository, IAdminService adminService)
    {
        _accountRepository = accountRepository;
        _thoughtService = thoughtService;
        _adminService = adminService;
        _verificationService = verificationService;
        _thoughtRepository = thoughtRepository;
    }

    [Authorize]
    public async Task<IActionResult> Index()
    {
        if (!User.HasClaim(ClaimTypes.Role, "Admin")) return Unauthorized();
        
        var vm = new AdminDashboardViewModel()
        {
            TotalUsers = (await _accountRepository.GetAll()).Count,
            TotalBannedUsers = 0,
            TotalThoughts = (await _thoughtService.GetThoughts()).Count,
            VerificationRequests = (await _accountRepository.GetVerificationRequests()).Count
        };

        return View(vm);
    }
    
    [HttpPost("[controller]/[action]/")]
    [Authorize]
    public async Task<IActionResult> PurgeUsers()
    {
        if (!User.HasClaim(ClaimTypes.Role, "Admin")) return Unauthorized();
        
        await _adminService.PurgeAllUsers();
        return RedirectToAction("Index");
    }
    
    [HttpPost("[controller]/[action]/")]
    [Authorize]
    public async Task<IActionResult> PurgeThoughts()
    {
        if (!User.HasClaim(ClaimTypes.Role, "Admin")) return Unauthorized();
        
        await _adminService.PurgeAllThoughts();
        return RedirectToAction("Index");
    }

    [Authorize]
    public async Task<IActionResult> Verifications()
    {
        if (!User.HasClaim(ClaimTypes.Role, "Admin")) return Unauthorized();

        var vm = new AdminVerificationsViewModel()
        {
            Requests = await _accountRepository.GetVerificationRequests()
        };
        
        return View(vm);
    }
    
    [HttpPost]
    [Route("[controller]/[action]/verify")]
    [Authorize]
    public async Task<IActionResult> Verify(int id, int status)
    {
        if (!User.HasClaim(ClaimTypes.Role, "Admin")) return Unauthorized();

        var result = await _verificationService.SetVerificationStatus(id, status);
        if (!result) return BadRequest();
        
        return RedirectToAction("Verifications");
    }
    
    [Authorize]
    public async Task<IActionResult> Content()
    {
        if (!User.HasClaim(ClaimTypes.Role, "Admin")) return Unauthorized();
        
        var vm = new AdminContentViewModel()
        {
            Thoughts = (List<Thought>?)await _thoughtRepository.GetThoughts()
        };

        return View(vm);
    }
    
    [Authorize]
    public async Task<IActionResult> Users()
    {
        if (!User.HasClaim(ClaimTypes.Role, "Admin")) return Unauthorized();
        
        var vm = new AdminUsersViewModel()
        {
            Users  = await _accountRepository.GetAll()
        };
        return View(vm);
    }
    
    [Authorize]
    public async Task<IActionResult> Bans()
    {
        if (!User.HasClaim(ClaimTypes.Role, "Admin")) return Unauthorized();

        var vm = new AdminUsersViewModel()
        {
            Users  = await _accountRepository.GetAll()
        };
        
        return View(vm);
    }
    
    [Authorize]
    public IActionResult Settings()
    {
        if (!User.HasClaim(ClaimTypes.Role, "Admin")) return Unauthorized();

        return View();
    }
}