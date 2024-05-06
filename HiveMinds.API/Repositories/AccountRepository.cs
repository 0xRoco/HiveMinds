using HiveMinds.API.Interfaces;
using HiveMinds.Common;
using HiveMinds.Database;
using HiveMinds.Models;
using Microsoft.EntityFrameworkCore;

namespace HiveMinds.API.Repositories;

public class AccountRepository : IAccountRepository
{
    
    private readonly DatabaseContext _db;
    private readonly DbSet<Account> _users;

    public AccountRepository(DatabaseContext db)
    {
        _db = db;
        _users = _db.Set<Account>();
    }

    public async Task<IEnumerable<Account>> GetAll()
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

    public async Task<Account?> GetByUsername(string username)
    {
        return await _users.AsNoTracking().FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<bool> CreateAccount(Account account)
    {
        await _users.AddAsync(account);
        var result = await _db.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> UpdateAccount(Account account, bool changeUpdateTime = true)
    {
        if (changeUpdateTime) await ChangeUpdateTime(account);
        _db.Entry(account).State = EntityState.Modified;
        var result = await _db.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> DeleteAccount(Account account)
    {
        var targetAccount = await GetByUsername(account.Username);
        if (targetAccount is not { Id: > 0 }) return false;

        account.Status = AccountStatus.Deactivated;
        account.DeletedAt = DateTime.UtcNow;

        _db.Entry(account).State = EntityState.Modified;
        
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