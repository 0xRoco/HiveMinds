using HiveMinds.Models;
using HiveMinds.ViewModels;

namespace HiveMinds.Services.Interfaces;

public interface IThoughtService
{
    Task<List<ThoughtViewModel>> GetThoughts();
    Task<ThoughtViewModel?> GetThought(int id);
    Task<List<ThoughtViewModel>?> GetThoughtsByUsername(string username);
    
    Task<bool> CreateThought(string username, string body);
    Task<bool> DeleteThought(int thoughtId);
    
    Task<List<ThoughtReplyViewModel>?> GetRepliesByThoughtId(int thoughtId);
    Task<List<ThoughtReplyViewModel>?> GetRepliesByUserId(int userId);
    Task<bool> ReplyToThought(int thoughtId, string username, string body);
    Task<bool> UpdateReply(int replyId, string body);
    Task<bool> DeleteReply(int replyId);
    
    Task<List<ThoughtLike>?> GetLikesByThoughtId(int thoughtId);
    Task<List<ThoughtLike>?> GetLikesByUserId(int userId);
    Task<bool> LikeThought(int thoughtId, string username);
    Task<bool> UnlikeThought(int thoughtId, string username);
}