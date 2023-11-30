using HiveMinds.DTO;
using HiveMinds.ViewModels;

namespace HiveMinds.Adapters.Interfaces;

public interface IModelToViewModelAdapter
{
    Task<UserViewModel> GetUserViewModel(UserDto account);
    Task<ThoughtViewModel> GetThoughtViewModel(ThoughtDto thought);
    Task<ReplyViewModel> GetThoughtReplyViewModel(ReplyDto reply);
}