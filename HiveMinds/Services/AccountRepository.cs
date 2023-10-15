using HiveMinds.Database;
using HiveMinds.Models;
using HiveMinds.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HiveMinds.Services;

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

    public async Task<List<Account>> GetAll()
    {
        return await _users.AsNoTracking().ToListAsync();
    }

    public async Task<Account?> GetById(int id)
    {
        return await _users.FindAsync(id);
    }

    public async Task<Account?> GetByEmail(string email)
    {
        return await _users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<Account?> GetByToken(string token)
    {
        return await _users.FirstOrDefaultAsync(u => u.LoginToken == token);
    }

    public Account? GetByUsername(string username)
    {
        return _users.FirstOrDefault(u => u.Username == username);
    }

    public bool Exists(string username)
    {
        return _users.Any(u => u.Username == username);
    }

    public async Task<bool> CreateUser(Account account)
    {
        await _users.AddAsync(account);
        var result = await _db.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> UpdateUser(Account account, bool changeUpdateTime = true)
    {
        _db.Entry(account).State = EntityState.Modified;
        var result = await _db.SaveChangesAsync();
        if (changeUpdateTime) await ChangeUpdateTime(account);
        return result > 0;
    }

    /// <summary>
    /// ONLY USE THIS IF YOU KNOW WHAT YOU'RE DOING.
    /// </summary>
    public async Task<bool> DeleteUser(Account account)
    {
        _users.Remove(account);
        var result = await _db.SaveChangesAsync();
        return result > 0;
    }

    public async Task<List<VerificationRequest>> GetVerificationRequests()
    {
        return await _verificationRequests.ToListAsync();
    }

    public async Task<VerificationRequest?> GetVerificationRequestById(int id)
    {
        return await _verificationRequests.FindAsync(id) ?? null;
    }

    public async Task<VerificationRequest?> GetVerificationRequestsByUserId(int userId)
    {
        return await _verificationRequests.FirstOrDefaultAsync(v => v.UserId == userId) ?? null;
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
        account.LastLoginAt = DateTime.UtcNow;
        await UpdateUser(account);    }
    
    private async Task ChangeUpdateTime(Account account)
    {
        account.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
    }
}