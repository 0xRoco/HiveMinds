using AutoMapper;
using HiveMinds.Adapters.Interfaces;
using HiveMinds.Models;
using HiveMinds.Services.Interfaces;
using HiveMinds.ViewModels;

namespace HiveMinds.Adapters;

public class ModelToViewModelAdapter : IModelToViewModelAdapter
{
    private const string DefaultUsername = "%MISSING_USERNAME%";
    private readonly IMapper _mapper;
    private readonly IThoughtRepository _thoughtRepository;
    private readonly IThoughtService _thoughtService;
    private readonly IAccountRepository _accountRepository;

    public ModelToViewModelAdapter(IMapper mapper, IThoughtRepository thoughtRepository, IAccountRepository accountRepository, IThoughtService thoughtService)
    {
        _mapper = mapper;
        _thoughtRepository = thoughtRepository;
        _accountRepository = accountRepository;
        _thoughtService = thoughtService;
    }
    
    public async Task<UserViewModel> GetUserViewModel(Account account)
    {
        var thoughts = await _thoughtService.GetThoughtsByUsername(account.Username);
        var likes = await _thoughtRepository.GetLikesForUser(account.Id);

        var vm = _mapper.Map<UserViewModel>(account);
        vm.Thoughts = thoughts;
        vm.Likes = likes;
        
        return vm;
    }

    public async Task<ThoughtViewModel> GetThoughtViewModel(Thought thought)
    {
        var thoughtAuthor = await _accountRepository.GetById(thought.UserId);
        var replies = await _thoughtService.GetRepliesByThoughtId(thought.Id);
        var likes = await _thoughtRepository.GetLikesByThoughtId(thought.Id);
        
        var vm = _mapper.Map<ThoughtViewModel>(thought);
        vm.Replies = replies;
        vm.Likes = likes;
        vm.Username = thoughtAuthor?.Username ?? DefaultUsername;
        
        return vm;    
    }

    public async Task<ThoughtReplyViewModel> GetThoughtReplyViewModel(ThoughtReply reply)
    {
        var replyAccount = await _accountRepository.GetById(reply.UserId);
        var vm = _mapper.Map<ThoughtReplyViewModel>(reply);
        vm.Username = replyAccount?.Username ?? DefaultUsername;
        
        return vm;
    }
}