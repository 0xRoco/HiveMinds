using System.Security.Cryptography;
using HiveMinds.Models;
using HiveMinds.Services.Interfaces;
using HiveMinds.ViewModels;

namespace HiveMinds.Services;

public class ThoughtService : IThoughtService
{
    private readonly IAccountRepository _account;
    private readonly IThoughtRepository _thought;
    private readonly IUtility _utility;
    
    public ThoughtService(IAccountRepository account, IThoughtRepository thought, IUtility utility)
    {
        _account = account;
        _thought = thought;
        _utility = utility;
    }
    
    public async Task<List<ThoughtViewModel>> GetThoughts()
    {
        var thoughtModels = await _thought.GetThoughts();
        if (thoughtModels == null) return new List<ThoughtViewModel>();
        var thoughtsViewModel = new List<ThoughtViewModel>();
        
        foreach (var thoughtModel in thoughtModels)
        {
            var user = await _account.GetById(thoughtModel.UserId);
            if (user == null) continue;
            var thoughtViewModel = new ThoughtViewModel
            {
                Id = thoughtModel.Id,
                Content = thoughtModel.Content,
                Username = user.Username,
                Likes = await GetLikesByThoughtId(thoughtModel.Id),
                Replies = await GetRepliesByThoughtId(thoughtModel.Id),
                CreatedAt = thoughtModel.CreatedAt
            };
            thoughtsViewModel.Add(thoughtViewModel);
        }

        thoughtsViewModel.Sort((x, y) => DateTime.Compare(y.CreatedAt, x.CreatedAt));
        return thoughtsViewModel;

    }
    
    public async Task<ThoughtViewModel?> GetThought(int id)
    {
        var thoughtModel = await _thought.GetThoughtById(id);
        if (thoughtModel == null) return null;
        
        var user = await _account.GetById(thoughtModel.UserId);
        if (user == null) return null;
        
        var thoughtViewModel = new ThoughtViewModel
        {
            Id = thoughtModel.Id,
            Content = thoughtModel.Content,
            Username = user.Username,
            Likes = await GetLikesByThoughtId(thoughtModel.Id),
            Replies = await GetRepliesByThoughtId(thoughtModel.Id),
            CreatedAt = thoughtModel.CreatedAt
        };
        
        return thoughtViewModel;
    }
    
    public async Task<List<ThoughtViewModel>?> GetThoughtsByUsername(string username)
    {
        var user = _account.GetByUsername(username);
        if (user == null) return null;
        var thoughtModels = await _thought.GetThoughtsByUserId(user.Id);
        if (thoughtModels == null) return null;
        
        var thoughtsViewModel = new List<ThoughtViewModel>();
        
        foreach (var thought in thoughtModels)
        {
            var thoughtViewModel = new ThoughtViewModel
            {
                Id = thought.Id,
                Username = user.Username,
                Content = thought.Content,
                Likes = await GetLikesByThoughtId(thought.Id),
                Replies = await GetRepliesByThoughtId(thought.Id),
                CreatedAt = thought.CreatedAt
            };
            thoughtsViewModel.Add(thoughtViewModel);
        }

        thoughtsViewModel.Sort((x, y) => DateTime.Compare(y.CreatedAt, x.CreatedAt));
        return thoughtsViewModel;
    }

    
    public async Task<bool> CreateThought(string username, string body)
    {
        var user = _account.GetByUsername(username);
        if (user == null) return false;
        var thought = new Thought
        {
            Id = RandomNumberGenerator.GetInt32(100000000,
                999999999),
            ParentThoughtId = -1,
            UserId = user.Id,
            Content = body,
            Likes = 0,
            CreatedAt = DateTime.UtcNow,
            Flagged = false
        };
        
        var result = await _thought.CreateThought(thought);

        return result;
    }

    public async Task<bool> DeleteThought(int thoughtId)
    {
        var thought = await _thought.GetThoughtById(thoughtId);
        if (thought == null) return false;
        var result = await _thought.DeleteThought(thoughtId);
        return result;
    }

    public async Task<List<ThoughtReplyViewModel>?> GetRepliesByThoughtId(int thoughtId)
    {
        var replies = await _thought.GetRepliesByThoughtId(thoughtId);
        if (replies == null) return null;
        var repliesViewModel = new List<ThoughtReplyViewModel>();
        foreach (var reply in replies)
        {
            var user = await _account.GetById(reply.UserId);
            if (user == null) continue;
            var replyViewModel = new ThoughtReplyViewModel
            {
                Id = reply.Id,
                Username = user.Username,
                Content = reply.Content,
                CreatedAt = reply.CreatedAt
            };
            repliesViewModel.Add(replyViewModel);
        }

        repliesViewModel.Sort((x, y) => DateTime.Compare(y.CreatedAt, x.CreatedAt));
        return repliesViewModel;
    }

    public async  Task<List<ThoughtReplyViewModel>?> GetRepliesByUserId(int userId)
    {
        var user = await _account.GetById(userId);
        if (user == null) return null;
        var replies = await _thought.GetRepliesForUser(userId);
        if (replies == null) return null;
        var repliesViewModel = replies.Select(reply => new ThoughtReplyViewModel
            {
                Id = reply.Id,
                Username = user.Username,
                Content = reply.Content,
                CreatedAt = reply.CreatedAt
            })
            .ToList();

        repliesViewModel.Sort((x, y) => DateTime.Compare(y.CreatedAt, x.CreatedAt));
        return repliesViewModel;
    }

    public async Task<bool> ReplyToThought(int thoughtId,string username, string body)
    {
        var thought = await _thought.GetThoughtById(thoughtId);
        var user = _account.GetByUsername(username);
        if (thought == null || user == null) return false;
        var reply = new ThoughtReply
        {
            Id = RandomNumberGenerator.GetInt32(100000000, 999999999),
            ThoughtId = thought.Id,
            UserId = user.Id,
            Content = body,
            CreatedAt = DateTime.UtcNow
        };
        
        var result = await _thought.CreateReply(reply);
        
        return result;
    }

    public Task<bool> UpdateReply(int replyId, string body)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteReply(int replyId)
    {
        throw new NotImplementedException();
    }
    
    public async Task<List<ThoughtLike>?> GetLikesByThoughtId(int thoughtId)
    {
        var likes = await _thought.GetLikesByThoughtId(thoughtId);
        return likes;
    }
    
    public async Task<List<ThoughtLike>?> GetLikesByUserId(int userId)
    {
        var likes = await _thought.GetLikesForUser(userId);
        return likes;
    }
    public async Task<bool> LikeThought(int thoughtId, string username)
    {
        var thought = await _thought.GetThoughtById(thoughtId);
        var user = _account.GetByUsername(username);
        if (thought == null || user == null) return false;
        if (await LikeExists(thoughtId, user.Id)) return false;
        
        var like = new ThoughtLike
        {
            Id = RandomNumberGenerator.GetInt32(100000000, 999999999),
            ThoughtId = thoughtId,
            CreatedAt = DateTime.UtcNow,
            UserId = user.Id
        };
        
        var result = await _thought.CreateLike(like);
        return result;
    }
    
    public async Task<bool> UnlikeThought(int thoughtId, string username)
    {
        var thought = await _thought.GetThoughtById(thoughtId);
        var user = _account.GetByUsername(username);
        if (thought == null || user == null) return false;
        if (!await LikeExists(thoughtId, user.Id)) return false;
        
        var likes = await _thought.GetLikesByThoughtId(thoughtId);
        
        var like = likes?.FirstOrDefault(x=> x.UserId == user.Id);
        if (like == null) return false;
        
        var result = await _thought.DeleteLike(like.Id);
        
        return result;
    }
    
    private async Task<bool> LikeExists(int thoughtId, int userId)
    {
        var likes = await _thought.GetLikesByThoughtId(thoughtId);
        return likes != null && likes.Any(like => like.UserId == userId);
    }
    
}