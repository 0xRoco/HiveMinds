using System.Security.Cryptography;
using AutoMapper;
using HiveMinds.API.Services.Interfaces;
using HiveMinds.DTO;
using HiveMinds.Models;

namespace HiveMinds.API.Services;

public class ThoughtService : IThoughtService
{
    private readonly IThoughtRepository _thoughtRepository;
    private readonly IAccountRepository _accountRepository;
    private readonly IMapper _mapper;


    public ThoughtService(IThoughtRepository thoughtRepository, IAccountRepository accountRepository, IMapper mapper)
    {
        _thoughtRepository = thoughtRepository;
        _accountRepository = accountRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ThoughtDto>> GetThoughts()
    {
        var thoughts = await _thoughtRepository.GetThoughts();
        if (thoughts is null) return new List<ThoughtDto>();
        
        var thoughtDtos = new List<ThoughtDto>();
        
        foreach (var thought in thoughts)
        {
            var thoughtDto = await GetThoughtById(thought.Id);
            if (thoughtDto is null) continue;
            thoughtDtos.Add(thoughtDto);
        }
        
        return thoughtDtos;
    }

    public async Task<ThoughtDto?> GetThoughtById(int id)
    {
        var thought = await _thoughtRepository.GetThoughtById(id);
        if (thought is null) return null;
        
        var replies = await GetRepliesByThoughtId(id);
        var likes = await GetLikesByThoughtId(id);
        
        var thoughtDto = _mapper.Map<ThoughtDto>(thought);

        thoughtDto.Replies = replies;
        thoughtDto.Likes = likes;
        
        var account = await _accountRepository.GetById(thought.UserId);
        
        thoughtDto.User = _mapper.Map<UserDto>(account);
        
        return thoughtDto;
    }

    public async Task<IEnumerable<ThoughtDto>> GetThoughtsByUsername(string username)
    {
        var account = await _accountRepository.GetByUsername(username);
        if (account is null) return new List<ThoughtDto>();

        var thoughts = await _thoughtRepository.GetThoughtsByUserId(account.Id);
        var thoughtDtos = new List<ThoughtDto>();

        if (thoughts is null) return thoughtDtos;
        
        foreach (var thought in thoughts)
        {
            var thoughtDto = await GetThoughtById(thought.Id);
            if (thoughtDto is null) continue;
            thoughtDtos.Add(thoughtDto);
        }

        return thoughtDtos;
    }

    public async Task<bool> CreateThought(string username, string body)
    {
        var account = await _accountRepository.GetByUsername(username);
        if (account is null) return false;
        
        var thought = new Thought
        {
            Id = RandomNumberGenerator.GetInt32(100000000, 999999999),
            ParentThoughtId = -1,
            UserId = account.Id,
            Content = body,
            CreatedAt = DateTime.UtcNow,
            Flagged = false,
        };

        return await _thoughtRepository.CreateThought(thought);
    }

    public async Task<bool> DeleteThought(int thoughtId)
    {
        return await _thoughtRepository.DeleteThought(thoughtId);
    }

    public async Task<ReplyDto?> GetReplyById(int replyId)
    {
        var reply = await _thoughtRepository.GetReplyById(replyId);

        if (reply is null) return null;
        
        var replyDto = _mapper.Map<ReplyDto>(reply);
        
        var user = await _accountRepository.GetById(reply.UserId);
        
        replyDto.User = _mapper.Map<UserDto>(user);
        
        return replyDto;
    }

    public async Task<IEnumerable<ReplyDto>> GetRepliesByThoughtId(int thoughtId)
    {
        var replies = await _thoughtRepository.GetRepliesByThoughtId(thoughtId);
        var replyDtos = new List<ReplyDto>();
        
        foreach (var reply in replies)
        {
            var replyDto = await GetReplyById(reply.Id);
            if (replyDto != null) replyDtos.Add(replyDto);
        }
        
        return replyDtos;
    }

    public async Task<IEnumerable<ReplyDto>> GetRepliesForUser(string username)
    {
        var account = await _accountRepository.GetByUsername(username);
        if (account is null) return new List<ReplyDto>();
        var replies = await _thoughtRepository.GetRepliesForUser(account.Id);
        
        var replyDtos = new List<ReplyDto>();
        
        foreach (var reply in replies)
        {
            var replyDto = await GetReplyById(reply.Id);
            if (replyDto != null) replyDtos.Add(replyDto);
        }
        
        return replyDtos;
    }

    public async Task<bool> ReplyToThought(int thoughtId, string username, string body)
    {
        var account = await _accountRepository.GetByUsername(username);
        if (account is null) return false;
        
        var reply = new ThoughtReply
        {
            Id = RandomNumberGenerator.GetInt32(100000000,
                999999999),
            ThoughtId = thoughtId,
            UserId = account.Id,
            Content = body,
            CreatedAt = DateTime.UtcNow,
        };

        return await _thoughtRepository.CreateReply(reply);
    }

    public async Task<bool> DeleteReply(int replyId)
    {
        return await _thoughtRepository.DeleteReply(replyId);
    }

    public async Task<LikeDto?> GetLikeById(int likeId)
    {
        var like = await _thoughtRepository.GetLikeById(likeId);
        if (like is null) return null;
        
        var likeDto = _mapper.Map<LikeDto>(like);
        var user = await _accountRepository.GetById(like.UserId);
        
        likeDto.User = _mapper.Map<UserDto>(user);
        
        return likeDto;
    }

    public async Task<IEnumerable<LikeDto>> GetLikesByThoughtId(int thoughtId)
    {
        var likes = await _thoughtRepository.GetLikesByThoughtId(thoughtId);
        var likeDtos = new List<LikeDto>();
        
        foreach (var like in likes)
        {
            var likeDto = await GetLikeById(like.Id);
            if (likeDto != null) likeDtos.Add(likeDto);
        }
        
        return likeDtos;
    }

    public async Task<IEnumerable<LikeDto>> GetLikesForUser(int userId)
    {
        var likes = await _thoughtRepository.GetLikesForUser(userId);
        
        var likeDtos = new List<LikeDto>();
        
        foreach (var like in likes)
        {
            var likeDto = await GetLikeById(like.Id);
            if (likeDto != null) likeDtos.Add(likeDto);
        }
        
        return likeDtos;
    }

    public async Task<bool> LikeThought(int thoughtId, string username)
    {
        var thought = await _thoughtRepository.GetThoughtById(thoughtId);
        var user = await _accountRepository.GetByUsername(username);
        if (thought is null || user is null) return false;
        if (await LikeExists(thoughtId, user.Id)) return false;
        
        var like = new ThoughtLike
        {
            Id = RandomNumberGenerator.GetInt32(100000000, 999999999),
            ThoughtId = thoughtId,
            CreatedAt = DateTime.UtcNow,
            UserId = user.Id
        };
        
        var result = await _thoughtRepository.CreateLike(like);
        return result;
    }

    public async Task<bool> UnlikeThought(int thoughtId, string username)
    {
        var thought = await _thoughtRepository.GetThoughtById(thoughtId);
        var user = await _accountRepository.GetByUsername(username);
        if (thought == null || user == null) return false;
        if (!await LikeExists(thoughtId, user.Id)) return false;
        
        var likes = await _thoughtRepository.GetLikesByThoughtId(thoughtId);
        
        var like = likes.FirstOrDefault(x=> x.UserId == user.Id);
        if (like == null) return false;
        
        var result = await _thoughtRepository.DeleteLike(like.Id);
        
        return result;
    }
    
    private async Task<bool> LikeExists(int thoughtId, int userId)
    {
        var likes = await _thoughtRepository.GetLikesByThoughtId(thoughtId);
        return likes.Any(like => like.UserId == userId);
    }
}