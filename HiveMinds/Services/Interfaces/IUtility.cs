using HiveMinds.DataTypes;
using HiveMinds.Models;
using HiveMinds.ViewModels;

namespace HiveMinds.Services.Interfaces;

public interface IUtility
{
    Task<UserViewModel> GetUserViewModel(Account account);
    Task<ThoughtViewModel> GetThoughtViewModel(Thought thought);
    bool IsUserVerified(string username);
    
    bool IsUserBanned(string username);
    bool IsUserAdmin(string username);
    
    Task<VerificationStatus> GetUserVerificationStatus(string username); }