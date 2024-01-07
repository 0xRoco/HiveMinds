using HiveMinds.Common;
using HiveMinds.DTO;
using HiveMinds.Interfaces;
using NuGet.Protocol;

namespace HiveMinds.Services;

public class AuthService : IAuthService
{
    private readonly ILogger<AuthService> _logger;

    private readonly HttpClient _httpClient;
    
    public AuthService(ILogger<AuthService> logger,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient("HiveMindsAPI");
    }

    public async Task<bool> Signup(SignupDto model, bool login = false)
    {
        var response = await _httpClient.PostAsJsonAsync("Auth/Signup", model);
        return response.IsSuccessStatusCode;
    }

    public async Task<ApiResponse<LoginResponseDto>?> Login(LoginDto model)
    {
        var response = await _httpClient.PostAsJsonAsync("Auth/Login", model);
        _logger.LogDebug($"{response.ToJson()}");
        if (!response.IsSuccessStatusCode) return null;
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<LoginResponseDto>>();
        return apiResponse;
    }

    public async Task<bool> Logout()
    {
        throw new NotImplementedException();
    }

    public async Task<bool> ResetPassword(string username, string token, string newPassword)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> SendPasswordReset(string username)
    {
        return false;
    }
}