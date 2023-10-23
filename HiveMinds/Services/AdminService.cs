using HiveMinds.Services.Interfaces;

namespace HiveMinds.Services;

public class AdminService : IAdminService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IThoughtRepository _thoughtRepository;


    public AdminService(IAccountRepository accountRepository, IThoughtRepository thoughtRepository)
    {
        _accountRepository = accountRepository;
        _thoughtRepository = thoughtRepository;
    }

    public async Task PurgeAllUsers()
    {
        foreach (var user in await _accountRepository.GetAll())
        {
            if (user.IsAdmin) continue;
            await _accountRepository.DeleteUser(user);
        }
    }

    public async Task<bool> BanUser(string username)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> UnbanUser(string username)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> VerifyUser(string username)
    {
        var account = await _accountRepository.GetByUsername(username);
        if (account == null) return false;
        account.IsVerified = true;
        return await _accountRepository.UpdateUser(account);
    }

    public async Task<bool> UnVerifyUser(string username)
    {
        var account = await _accountRepository.GetByUsername(username);
        if (account == null) return false;
        account.IsVerified = false;
        return await _accountRepository.UpdateUser(account);
    }

    public async Task<bool> PurgeAllBans()
    {
        throw new NotImplementedException();
    }

    public async Task PurgeAllThoughts()
    {
        foreach (var thought in (await _thoughtRepository.GetThoughts())!)
        {
            var replies = await _thoughtRepository.GetRepliesByThoughtId(thought.Id);
            var likes = await _thoughtRepository.GetLikesByThoughtId(thought.Id);

            if (replies != null)
            {
                foreach (var reply in replies)
                {
                    await _thoughtRepository.DeleteReply(reply.Id);
                }
            }

            if (likes != null)
            {
                foreach (var like in likes)
                {
                    await _thoughtRepository.DeleteLike(like.Id);
                }
            }
            
            await _thoughtRepository.DeleteThought(thought.Id);
        }
    }
}