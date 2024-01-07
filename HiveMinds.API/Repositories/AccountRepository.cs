using HiveMinds.API.Interfaces;
using HiveMinds.Database;
using HiveMinds.Models;
using Microsoft.EntityFrameworkCore;

namespace HiveMinds.API.Repositories;

public class AccountRepository : IAccountRepository
{
    
    private readonly DatabaseContext _db;
    private readonly DbSet<Account> _users;
    private readonly DbSet<VerificationRequest> _verificationRequests;
    private readonly ILogger<AccountRepository> _logger;
    
    public AccountRepository(DatabaseContext db, ILogger<AccountRepository> logger)
    {
        _db = db;
        _users = _db.Set<Account>();
        _verificationRequests = _db.Set<VerificationRequest>();
        _logger = logger;
    }

    public async Task<IEnumerable<Account>> GetAll()
    {
        return await _users.AsNoTracking().ToListAsync();
    }

    public async Task<IEnumerable<Account>> GetByIds(List<int> ids)
    {
        if (ids.Count == 0) return new List<Account>();
        return await _users.AsNoTracking().Where(u => ids.Contains(u.Id)).ToListAsync();
    }
    
    public async Task<Account?> GetById(int id)
    {
        return await _users.FindAsync(id);
    }

    public async Task<Account?> GetByEmail(string email)
    {
        return await _users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<Account?> GetByUsername(string username)
    {
        return await _users.AsNoTracking().FirstOrDefaultAsync(u => u.Username == username);
    }

    [Obsolete("Use GetByUsername/Id instead to avoid an extra database call", true)]
    public async Task<bool> Exists(string username)
    {
        return await _users.AsNoTracking().AnyAsync(u => u.Username == username);
    }

    public async Task<bool> CreateUser(Account account)
    {
        await _users.AddAsync(account);
        var result = await _db.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> UpdateUser(Account account, bool changeUpdateTime = true)
    {
        if (changeUpdateTime) await ChangeUpdateTime(account);
        _db.Entry(account).State = EntityState.Modified;
        var result = await _db.SaveChangesAsync();
        return result > 0;
    }

    /*
     this method should set the status of the user to deactivated instead of fully deleting it unless deleting is the intended action?
     fully deleting a user causes headaches because then we need to also delete all the user's posts, comments, etc.
     */
    public async Task<bool> DeleteUser(Account account)
    {
        var targetAccount = await GetByUsername(account.Username);
        if (string.IsNullOrEmpty(targetAccount?.Username)) return false;

        _db.Entry(targetAccount).State = EntityState.Deleted;
        _users.Remove(account);
        var result = await _db.SaveChangesAsync();
        return result > 0;
    }

    public async Task<IEnumerable<VerificationRequest>> GetVerificationRequests()
    {
        return await _verificationRequests.AsNoTracking().ToListAsync();
    }

    public async Task<VerificationRequest?> GetVerificationRequestById(int id)
    {
        return await _verificationRequests.FindAsync(id);
    }

    public async Task<VerificationRequest?> GetVerificationRequestsByUserId(int userId)
    {
        return await _verificationRequests.AsNoTracking().FirstOrDefaultAsync(v => v.UserId == userId);
    }

    public async Task<bool> CreateVerificationRequest(VerificationRequest request)
    {
        await _verificationRequests.AddAsync(request);
        var result = await _db.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> UpdateVerificationRequest(VerificationRequest request)
    {
        _db.Entry(request).State = EntityState.Modified;
        var result = await _db.SaveChangesAsync();
        return result > 0;
    }

    public async Task UpdateLastLogin(Account account)
    {
        _db.Entry(account).State = EntityState.Modified;
        account.LastLoginAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(); 
    }
    
    private async Task ChangeUpdateTime(Account account)
    {
        _db.Entry(account).State = EntityState.Modified;
        account.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }
}