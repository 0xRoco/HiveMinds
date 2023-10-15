using HiveMinds.Models;

namespace HiveMinds.Services.Interfaces;

public interface IAccountRepository
{
    Task<List<Account>> GetAll();
    Task<Account?> GetById(int id);
    Task<Account?> GetByEmail(string email);
    Task<Account?> GetByToken(string token);
    Account? GetByUsername(string username);
    bool Exists(string username);
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