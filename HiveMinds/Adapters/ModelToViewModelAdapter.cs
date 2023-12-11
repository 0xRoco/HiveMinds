using AutoMapper;
using HiveMinds.Adapters.Interfaces;
using HiveMinds.DTO;
using HiveMinds.Interfaces;
using HiveMinds.Models;
using HiveMinds.ViewModels;

namespace HiveMinds.Adapters;

public class ModelToViewModelAdapter : IModelToViewModelAdapter
{
    private const string DefaultUsername = "%MISSING_USERNAME%";
    private readonly IMapper _mapper;

    private readonly IUserService _userService;
    private readonly IThoughtService _thoughtService;

    public ModelToViewModelAdapter(IMapper mapper, IUserService userService, IThoughtService thoughtService)
    {
        _mapper = mapper;
        _userService = userService;
        _thoughtService = thoughtService;
    }

    public async Task<UserViewModel> GetUserViewModel(UserDto user)
    {
        var thoughts = await _thoughtService.GetThoughtsForUser(user.Username);
        var likes = await _thoughtService.GetLikesForUser(user.Username);

        var vm = _mapper.Map<UserViewModel>(user);
        vm.Thoughts = _mapper.Map<List<ThoughtViewModel>>(thoughts);
        vm.Likes = _mapper.Map<List<ThoughtLike>>(likes);
        
        return vm;
    }

    public async Task<ThoughtViewModel> GetThoughtViewModel(ThoughtDto thought)
    {
        var thoughtAuthor = thought.User;
        var replies = await _thoughtService.GetRepliesByThoughtId(thought.Id);
        var likes = await _thoughtService.GetLikesByThoughtId(thought.Id);
        
        var vm = _mapper.Map<ThoughtViewModel>(thought);
        vm.Replies = _mapper.Map<List<ReplyViewModel>>(replies);
        vm.Likes = _mapper.Map<List<ThoughtLike>>(likes);
        vm.Author = thoughtAuthor;
        
        return vm;    
    }

    public async Task<ReplyViewModel> GetThoughtReplyViewModel(ReplyDto reply)
    {
        var replyAccount = reply.User;
        var vm = _mapper.Map<ReplyViewModel>(reply);
        vm.Username = replyAccount.Username ?? DefaultUsername;
        
        return vm;
    }
}