using HiveMinds.Models;

namespace HiveMinds.API.Interfaces;

public interface IAccountRepository
{
    Task<IEnumerable<Account>> GetAll();
    Task<IEnumerable<Account>> GetByIds(List<int> ids);
    Task<Account?> GetById(int id);
    Task<Account?> GetByEmail(string email);
    Task<Account?> GetByUsername(string username);
    Task<bool> Exists(string username);
    Task<bool> CreateUser(Account account);
    Task<bool> UpdateUser(Account account, bool changeUpdateTime = true);
    Task<bool> DeleteUser(Account account);

    Task<IEnumerable<VerificationRequest>> GetVerificationRequests();
    Task<VerificationRequest?> GetVerificationRequestById(int id);
    Task<VerificationRequest?> GetVerificationRequestsByUserId(int userId);
    Task<bool> CreateVerificationRequest(VerificationRequest request);
    Task<bool> UpdateVerificationRequest(VerificationRequest request);
    
    Task UpdateLastLogin(Account account);
    
}