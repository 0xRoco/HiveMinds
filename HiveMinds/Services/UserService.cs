using HiveMinds.Common;
using HiveMinds.DTO;
using HiveMinds.Interfaces;

namespace HiveMinds.Services;

public class UserService : IUserService
{
    private readonly HttpClient _httpClient;

    public UserService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("HiveMindsAPI");
    }

    public async Task<ApiResponse<UserDto>?> GetUser(string username)
    {
        var response = await _httpClient.GetAsync($"Users/{username}");
        if (!response.IsSuccessStatusCode) return null;
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<UserDto>>();
        return apiResponse;
    }

    public async Task<ApiResponse<UserDto>?> GetUser(int id)
    {
        var response = await _httpClient.GetAsync($"Users/{id}");
        if (!response.IsSuccessStatusCode) return null;
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<UserDto>>();
        return apiResponse;
    }

    public async Task<ApiResponse<UserDto>?> UpdateUserProfile(string username, EditProfileDto model,
        IFormFile? profilePicture)
    {
        if (profilePicture is not null)
        {
            var pfpResponse = await _httpClient.PutAsync($"Profiles/{username}/profile-picture",
                new MultipartFormDataContent
                {
                    { new StreamContent(profilePicture!.OpenReadStream()), "profilePicture", profilePicture.FileName }
                });
            if (!pfpResponse.IsSuccessStatusCode) return null;
        }

        var response = await _httpClient.PutAsJsonAsync($"Profiles/{username}", model);
        if (!response.IsSuccessStatusCode) return null;

        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<UserDto>>();
        return apiResponse;
    }

    public async Task<ApiResponse<object>?> VerifyEmail(VerifyEmailDto model)
    {
        var response = await _httpClient.PostAsJsonAsync("Users/verify-email", model);
        if (!response.IsSuccessStatusCode) return null;
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
        return apiResponse;
    }

    public async Task<ApiResponse<VerificationDto>?> GetVerificationRequest(int accountId)
    {
        var response = await _httpClient.GetAsync($"Verification?accountId={accountId}");
        if (!response.IsSuccessStatusCode) return null;
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<VerificationDto>>();
        return apiResponse;
    }

    public async Task<ApiResponse<VerificationDto>?> GetVerificationRequest(string username)
    {
        var response = await _httpClient.GetAsync($"Verification?username={username}");
        if (!response.IsSuccessStatusCode) return null;
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<VerificationDto>>();
        return apiResponse;
    }

    public async Task<ApiResponse<VerificationDto>?> CreateVerificationRequest(VerificationRequestDto request)
    {
        var response = await _httpClient.PostAsJsonAsync("Verification", request);
        if (!response.IsSuccessStatusCode) return null;
        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<VerificationDto>>();
        return apiResponse;
    }
}