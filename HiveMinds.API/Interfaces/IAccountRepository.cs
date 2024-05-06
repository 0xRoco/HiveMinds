using HiveMinds.Models;

namespace HiveMinds.API.Interfaces;

public interface IAccountRepository
{
    Task<IEnumerable<Account>> GetAll();
    Task<Account?> GetById(int id);
    Task<Account?> GetByEmail(string email);
    Task<Account?> GetByUsername(string username);
    Task<bool> CreateAccount(Account account);
    Task<bool> UpdateAccount(Account account, bool changeUpdateTime = true);
    Task<bool> DeleteAccount(Account account);
    
    Task UpdateLastLogin(Account account);
    
}