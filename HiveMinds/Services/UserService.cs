using HiveMinds.DTO;
using HiveMinds.Services.Interfaces;

namespace HiveMinds.Services;

public class UserService : IUserService
{
    private readonly HttpClient _httpClient;

    public UserService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("HiveMindsAPI");
    }

    public async Task<UserDto?> GetUser(string username)
    {
        var response = await _httpClient.GetAsync($"Users/{username}");
        if (!response.IsSuccessStatusCode) return null;
        var userDto = await response.Content.ReadFromJsonAsync<UserDto>();
        return userDto;
    }

    public async Task<UserDto?> GetUser(int id)
    {
        var response = await _httpClient.GetAsync($"Users/{id}");
        if (!response.IsSuccessStatusCode) return null;
        var userDto = await response.Content.ReadFromJsonAsync<UserDto>();
        return userDto;
    }
}