using HiveMinds.Models;

namespace HiveMinds.API.Services.Interfaces;

public interface IAccountRepository
{
    Task<List<Account>> GetAll();
    Task<List<Account>> GetByIds(List<int> ids);
    Task<Account?> GetById(int id);
    Task<Account?> GetByEmail(string email);
    Task<Account?> GetByToken(string token);
    Task<Account?> GetByUsername(string username);
    Task<bool> Exists(string username);
    Task<bool> CreateUser(Account account);
    Task<bool> UpdateUser(Account account, bool changeUpdateTime = true);
    Task<bool> DeleteUser(Account account); 
    
    Task<List<VerificationRequest>> GetVerificationRequests();
    Task<VerificationRequest?> GetVerificationRequestById(int id);
    Task<VerificationRequest?> GetVerificationRequestsByUserId(int userId);
    Task<bool> CreateVerificationRequest(VerificationRequest request);
    Task<bool> UpdateVerificationRequest(VerificationRequest request);
    
    Task UpdateLastLogin(Account account);
    
}