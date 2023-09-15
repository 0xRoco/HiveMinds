using HiveMinds.Models;

namespace HiveMinds.Services.Interfaces;

public interface IThoughtRepository
{
    Task<List<Thought>?> GetThoughts();
    Task<Thought?> GetThoughtById(int id);
    Task<List<Thought>?> GetThoughtByUserId(int id);
    Task<bool> CreateThought(Thought thought);
    Task<bool> UpdateThought(Thought thought);
    Task<bool> DeleteThought(Thought thought);
    
    Task<List<ThoughtLike>?> GetLikesByThoughtId(int id);
    Task<List<ThoughtLike>?> GetLikesByUserId(int id);
    Task<bool> CreateLike(ThoughtLike thoughtLike);
    Task<bool> DeleteLike(ThoughtLike thoughtLike);
    
    Task<List<ThoughtReply>?> GetRepliesByThoughtId(int id);
    Task<List<ThoughtReply>?> GetRepliesByUserId(int id);
    Task<bool> CreateReply(ThoughtReply thoughtReply);
    Task<bool> UpdateReply(ThoughtReply thoughtReply);
    Task<bool> DeleteReply(ThoughtReply thoughtReply);
}