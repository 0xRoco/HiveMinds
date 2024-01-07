using HiveMinds.API.Services;
using HiveMinds.Common;
using HiveMinds.DTO;

namespace HiveMinds.API.Interfaces;

public interface IAuthService
{
    Task<Result<LoginResponseDto, AuthService.AuthResults>> Login(LoginDto loginDto);
    Task<Result<int, AuthService.AuthResults>> Signup(SignupDto signupDto);
}