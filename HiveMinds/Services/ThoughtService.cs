using AutoMapper;
using HiveMinds.DTO;
using HiveMinds.Services.Interfaces;

namespace HiveMinds.Services;

public class ThoughtService : IThoughtService
{
    private readonly HttpClient _httpClient;

    public ThoughtService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("HiveMindsAPI");
    }

    // Thoughts

    public async Task<IEnumerable<ThoughtDto>> GetThoughts()
    {
        var response = await _httpClient.GetAsync("Thoughts");
        var thoughtDtos = await response.Content.ReadFromJsonAsync<IEnumerable<ThoughtDto>>();
        return thoughtDtos ?? Enumerable.Empty<ThoughtDto>();
    }

    public async Task<ThoughtDto?> GetThought(int id)
    {
        var response = await _httpClient.GetAsync($"Thoughts/{id}");
        if (!response.IsSuccessStatusCode) return null;
        var thoughtDto = await response.Content.ReadFromJsonAsync<ThoughtDto>();
        return thoughtDto;
    }

    public async Task<IEnumerable<ThoughtDto>> GetThoughtsForUser(int userId)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<ThoughtDto>> GetThoughtsForUser(string username)
    {
        var response = await _httpClient.GetAsync($"Thoughts/{username}");
        var thoughtDtos = await response.Content.ReadFromJsonAsync<IEnumerable<ThoughtDto>>();
        return thoughtDtos ?? Enumerable.Empty<ThoughtDto>();
    }

    public async Task<bool> CreateThought(string username, string body)
    {
        var response = await _httpClient.PostAsJsonAsync($"Thoughts/{username}", body);
        return response.IsSuccessStatusCode;
    }

    // Replies

    public async Task<ReplyDto?> GetReplyById(int replyId)
    {
        var response = await _httpClient.GetAsync($"Replies/{replyId}");
        if (!response.IsSuccessStatusCode) return null;
        var replyDto = await response.Content.ReadFromJsonAsync<ReplyDto>();
        return replyDto ?? new ReplyDto();
    }

    public async Task<IEnumerable<ReplyDto>> GetRepliesByThoughtId(int thoughtId)
    {
        var response = await _httpClient.GetAsync($"Replies?thoughtId={thoughtId}");
        var replyDtos = await response.Content.ReadFromJsonAsync<IEnumerable<ReplyDto>>();
        return replyDtos ?? Enumerable.Empty<ReplyDto>();
    }

    public async Task<IEnumerable<ReplyDto>> GetRepliesForUser(int userId)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<ReplyDto>> GetRepliesForUser(string username)
    {
        var response = await _httpClient.GetAsync($"Replies/{username}");
        var replyDtos = await response.Content.ReadFromJsonAsync<IEnumerable<ReplyDto>>();
        return replyDtos ?? Enumerable.Empty<ReplyDto>();
    }

    public async Task<bool> ReplyToThought(int thoughtId, string username, string body)
    {
        var response = await _httpClient.PostAsJsonAsync($"Replies/{thoughtId}?username={username}", body);
        return response.IsSuccessStatusCode;
    }

    // Likes

    public async Task<IEnumerable<LikeDto>> GetLikesByThoughtId(int thoughtId)
    {
        var response = await _httpClient.GetAsync($"Likes?thoughtId={thoughtId}");
        var likeDtos = await response.Content.ReadFromJsonAsync<IEnumerable<LikeDto>>();
        return likeDtos ?? Enumerable.Empty<LikeDto>();
    }

    public async Task<IEnumerable<LikeDto>> GetLikesForUser(int userId)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<LikeDto>> GetLikesForUser(string username)
    {
        var response = await _httpClient.GetAsync($"Likes/{username}");
        var likeDtos = await response.Content.ReadFromJsonAsync<IEnumerable<LikeDto>>();
        return likeDtos ?? Enumerable.Empty<LikeDto>();
    }

    public async Task<bool> LikeThought(int thoughtId, string username)
    {
        var response = await _httpClient.PostAsJsonAsync($"Likes/{thoughtId}?username={username}", "");
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UnlikeThought(int thoughtId, string username)
    {
        var response = await _httpClient.DeleteAsync($"Likes/{thoughtId}?username={username}");
        return response.IsSuccessStatusCode;
    }
}