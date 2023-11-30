using HiveMinds.DTO;
using HiveMinds.Models;

namespace HiveMinds.API.Services.Interfaces;

public interface IAccountFactory
{
    Task<Account> CreateAccountModel(SignupDto model);
    Task<Account> CreateAccountModel(string username, string password);
    Task<Account> CreateDemoAccountModel();
}