using HiveMinds.DTO;
using HiveMinds.Models;

namespace HiveMinds.API.Interfaces;

public interface IAccountFactory
{
    Account CreateAccountModel(SignupDto model);
    Account CreateAccountModel(string username, string password);
    Account CreateDemoAccountModel();
}