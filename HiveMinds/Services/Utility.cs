using HiveMinds.DataTypes;
using HiveMinds.Models;
using HiveMinds.Services.Interfaces;
using HiveMinds.ViewModels;

namespace HiveMinds.Services;

public class Utility : IUtility
{
    private readonly Lazy<IThoughtService> _thoughtService;
    private readonly Lazy<IAccountRepository> _accountRepository;


    public Utility(Lazy<IThoughtService> thoughtService, Lazy<IAccountRepository> accountRepository)
    {
        _thoughtService = thoughtService;
        _accountRepository = accountRepository;
    }

    public async Task<UserViewModel> GetUserViewModel(Account account)
    {
        var thoughts = await _thoughtService.Value.GetThoughtsByUsername(account.Username);
        var likes = await _thoughtService.Value.GetLikesByUserId(account.Id);
        
        var profile = new UserViewModel()
        {
            Username = account.Username,
            PartyLoyaltyStatement = account.LoyaltyStatement,
            Bio = account.Bio,
            Thoughts = thoughts,
            Likes = likes,
            IsVerified = account.IsVerified,
            Joined = account.CreatedAt,
            LastSeen = account.UpdatedAt
        };

        return profile;
    }

    public async Task<ThoughtViewModel> GetThoughtViewModel(Thought thought)
    {
        var replies = await _thoughtService.Value.GetRepliesByThoughtId(thought.Id);
        var likes = await _thoughtService.Value.GetLikesByThoughtId(thought.Id);
        var account = await _accountRepository.Value.GetById(thought.UserId);
        
        var thoughtViewModel = new ThoughtViewModel()
        {
            Id = thought.Id,
            Username = account.Username,
            Content = thought.Content,
            Likes = likes,
            Replies = replies,
            CreatedAt = thought.CreatedAt
        };
        
        return thoughtViewModel;
    }

    public async Task<bool> IsUserVerified(string username)
    {
        var account = await _accountRepository.Value.GetByUsername(username);
        return account is { IsVerified: true };
    }

    public async Task<bool> IsUserBanned(string username)
    {
        var account = await _accountRepository.Value.GetByUsername(username);
        return account == null && false;
    }

    public async Task<bool> IsUserAdmin(string username)
    {
        var account =  await _accountRepository.Value.GetByUsername(username);
        return account is { IsAdmin: true };
    }

    public async Task<VerificationStatus> GetUserVerificationStatus(string username)
    {
        var account =  await _accountRepository.Value.GetByUsername(username);
        if (account == null) return VerificationStatus.None;
        var request = await _accountRepository.Value.GetVerificationRequestsByUserId(account.Id);
        return request?.Status ?? VerificationStatus.None;
    }
}