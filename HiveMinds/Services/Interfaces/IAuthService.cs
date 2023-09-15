using HiveMinds.Models;
using HiveMinds.ViewModels;

namespace HiveMinds.Services.Interfaces;

public interface IAuthService
{
    Task<bool> Signup(SignupViewModel model, bool login = false);
    Task<bool> Login(LoginViewModel model);
    Task<bool> Logout();
    Task<bool> ResetPassword(string username, string token, string newPassword);
    Task<bool> SendPasswordReset(string username);
}