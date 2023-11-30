using HiveMinds.DataTypes;
using HiveMinds.Models;
using HiveMinds.Services.Interfaces;
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
        var account = await _userService.GetUser(username);
        return account is { IsVerified: true };
    }

    public async Task<VerificationStatus> GetUserVerificationStatus(string username)
    {
        var account = await _userService.GetUser(username);
        if (account == null) return VerificationStatus.None;
        return VerificationStatus.None;
    }
}