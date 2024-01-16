using HiveMinds.DTO;

namespace HiveMinds.Interfaces;

public interface IThoughtService
{
    public Task<IEnumerable<ThoughtDto>> GetThoughts();
    Task<ThoughtDto?> GetThought(int id);
    Task<IEnumerable<ThoughtDto>> GetThoughtsForUser(int userId);
    Task<IEnumerable<ThoughtDto>> GetThoughtsForUser(string username);
    Task<bool> CreateThought(string body);

    Task<ReplyDto?> GetReplyById(int replyId);
    Task<IEnumerable<ReplyDto>> GetRepliesByThoughtId(int thoughtId);
    Task<IEnumerable<ReplyDto>> GetRepliesForUser(int userId);
    Task<IEnumerable<ReplyDto>> GetRepliesForUser(string username);
    Task<bool> ReplyToThought(int thoughtId, string username, string body);


    Task<IEnumerable<LikeDto>> GetLikesByThoughtId(int thoughtId);
    Task<IEnumerable<LikeDto>> GetLikesForUser(int userId);
    Task<IEnumerable<LikeDto>> GetLikesForUser(string username);
    Task<bool> LikeThought(int thoughtId);
    Task<bool> UnlikeThought(int thoughtId);
}