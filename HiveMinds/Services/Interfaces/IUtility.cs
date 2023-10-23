using HiveMinds.DataTypes;
using HiveMinds.Models;
using HiveMinds.ViewModels;

namespace HiveMinds.Services.Interfaces;

public interface IUtility
{
    [Obsolete("L", true)]
    Task<UserViewModel> GetUserViewModel(Account account);
    Task<ThoughtViewModel> GetThoughtViewModel(Thought thought);
    Task<bool> IsUserVerified(string username);
    
    Task<bool> IsUserBanned(string username);
    Task<bool> IsUserAdmin(string username);
    
    Task<VerificationStatus> GetUserVerificationStatus(string username); }