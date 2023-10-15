using HiveMinds.Models;
using HiveMinds.ViewModels;

namespace HiveMinds.Services.Interfaces;

public interface IAccountFactory
{
    Task<Account> CreateAccountModel(SignupViewModel model);
    Task<Account> CreateAccountModel(string username, string password);
    Task<Account> CreateDemoAccountModel();
}