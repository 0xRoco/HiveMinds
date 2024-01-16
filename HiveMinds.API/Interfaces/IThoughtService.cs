using HiveMinds.DTO;

namespace HiveMinds.API.Interfaces;

public interface IThoughtService
{
    public Task<IEnumerable<ThoughtDto>> GetThoughts();
    public Task<IEnumerable<ThoughtDto>> GetThoughtsByUsername(string username);
    public Task<ThoughtDto?> GetThoughtById(int id);
    
    public Task<bool> CreateThought(string username, string body);
    public Task<bool> DeleteThought(int thoughtId);

    public Task<ReplyDto?> GetReplyById(int replyId);
    public Task<IEnumerable<ReplyDto>> GetRepliesByThoughtId(int thoughtId);
    public Task<IEnumerable<ReplyDto>> GetRepliesForUser(string username);
    public Task<bool> ReplyToThought(int thoughtId, string username, string body);
    public Task<bool> DeleteReply(int replyId);

    public Task<LikeDto?> GetLikeById(int likeId);
    public Task<IEnumerable<LikeDto>> GetLikesByThoughtId(int thoughtId);
    public Task<IEnumerable<LikeDto>> GetLikesForUser(int accountId);
    public Task<bool> LikeThought(int thoughtId, int accountId);
    public Task<bool> UnlikeThought(int thoughtId, int accountId);
}