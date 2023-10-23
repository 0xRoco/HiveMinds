using HiveMinds.Models;
using HiveMinds.ViewModels;

namespace HiveMinds.Adapters.Interfaces;

public interface IModelToViewModelAdapter
{
    Task<UserViewModel> GetUserViewModel(Account account);
    Task<ThoughtViewModel> GetThoughtViewModel(Thought thought);
    Task<ThoughtReplyViewModel> GetThoughtReplyViewModel(ThoughtReply reply);
}