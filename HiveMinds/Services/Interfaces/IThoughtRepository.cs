using HiveMinds.Models;

namespace HiveMinds.Services.Interfaces;

public interface IThoughtRepository
{
    Task<IReadOnlyList<Thought>?> GetThoughts();
    Task<Thought?> GetThoughtById(int id);
    Task<List<Thought>?> GetThoughtsByUserId(int id);
    Task<bool> CreateThought(Thought thought);
    Task<bool> UpdateThought(Thought thought);
    Task<bool> DeleteThought(int thoughtId);
    
    Task<List<ThoughtLike>?> GetLikesByThoughtId(int id);
    Task<List<ThoughtLike>?> GetLikesForUser(int id);
    Task<bool> CreateLike(ThoughtLike thoughtLike);
    Task<bool> DeleteLike(int thoughtLikeId);
    
    Task<List<ThoughtReply>?> GetRepliesByThoughtId(int id);
    Task<List<ThoughtReply>?> GetRepliesForUser(int id);
    Task<bool> CreateReply(ThoughtReply thoughtReply);
    Task<bool> UpdateReply(ThoughtReply thoughtReply);
    Task<bool> DeleteReply(int thoughtReplyId);
}