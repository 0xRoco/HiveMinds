using HiveMinds.Common;
using HiveMinds.Interfaces;
using HiveMinds.Models;
using HiveMinds.ViewModels;

namespace HiveMinds.Services;

public class Utility : IUtility
{
    private readonly IUserService _userService;

    public Utility(IUserService userService)
    {
        _userService = userService;
    }
    
    public async Task<bool> IsUserVerified(string username)
    {
        var account = (await _userService.GetUser(username))?.Data;
        return account is { IsVerified: true };
    }

    public async Task<VerificationStatus> GetUserVerificationStatus(string username)
    {
        var account = (await _userService.GetUser(username))?.Data;
        if (account == null) return VerificationStatus.None;
        return VerificationStatus.None;
    }
}