using HiveMinds.Common;
using HiveMinds.DTO;

namespace HiveMinds.Interfaces;

public interface IAuthService
{
    Task<bool> Signup(SignupDto model, bool login = false);
    Task<ApiResponse<LoginResponseDto>?> Login(LoginDto model);
    Task<bool> Logout();
    Task<bool> ResetPassword(string username, string token, string newPassword);
    Task<bool> SendPasswordReset(string username);
}