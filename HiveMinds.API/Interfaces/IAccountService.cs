using HiveMinds.DTO;

namespace HiveMinds.API.Interfaces;

public interface IAccountService
{
    Task<IEnumerable<AccountDto>> GetAll();
    Task<IEnumerable<AccountDto>> GetByIds(IEnumerable<int> ids);

    Task<AccountDto?> GetById(int id);
    Task<AccountDto?> GetByUsername(string username);
    Task<AccountDto?> GetByEmail(string email);

    Task<bool> Exists(string username);

    Task<bool> Create(SignupDto account);
    Task<bool> Update(AccountDto account);
    Task<bool> Delete(AccountDto account);
}