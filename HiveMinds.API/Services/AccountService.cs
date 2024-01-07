using AutoMapper;
using HiveMinds.API.Interfaces;
using HiveMinds.Common;
using HiveMinds.DTO;
using HiveMinds.Models;

namespace HiveMinds.API.Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IAccountFactory _accountFactory;
    private readonly IMapper _mapper;

    public AccountService(IAccountRepository accountRepository, IMapper mapper, IAccountFactory accountFactory)
    {
        _accountRepository = accountRepository;
        _mapper = mapper;
        _accountFactory = accountFactory;
    }

    public async Task<IEnumerable<AccountDto>> GetAll()
    {
        var accounts = await _accountRepository.GetAll();

        var dtos = new List<AccountDto>();

        foreach (var account in accounts)
        {
            var dto = await MapAccountToDto(account);
            dtos.Add(dto);
        }

        return dtos;
    }

    public async Task<IEnumerable<AccountDto>> GetByIds(IEnumerable<int> ids)
    {
        var account = await _accountRepository.GetByIds(ids.ToList());
        var dtos = new List<AccountDto>();

        foreach (var a in account)
        {
            var dto = await MapAccountToDto(a);
            dtos.Add(dto);
        }

        return dtos;
    }

    public async Task<AccountDto?> GetById(int id)
    {
        var account = await _accountRepository.GetById(id);
        if (account == null) return null;

        var dto = await MapAccountToDto(account);

        return dto;
    }

    public async Task<AccountDto?> GetByUsername(string username)
    {
        var account = await _accountRepository.GetByUsername(username);
        if (account == null) return null;

        var dto = await MapAccountToDto(account);

        return dto;
    }

    public async Task<AccountDto?> GetByEmail(string email)
    {
        var account = await _accountRepository.GetByEmail(email);
        if (account == null) return null;

        var dto = await MapAccountToDto(account);

        return dto;
    }

    public async Task<bool> Exists(string username)
    {
        return await _accountRepository.Exists(username);
    }

    public async Task<bool> Create(SignupDto account)
    {
        if (await _accountRepository.Exists(account.Username)) return false;

        var accountModel = _accountFactory.CreateAccountModel(account);
        return await _accountRepository.CreateUser(accountModel);
    }

    public async Task<bool> Update(AccountDto account)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> Delete(AccountDto account)
    {
        throw new NotImplementedException();
    }

    private Task<AccountDto> MapAccountToDto(Account account)
    {
        var dto = _mapper.Map<AccountDto>(account);
        return Task.FromResult(dto);
    }
}