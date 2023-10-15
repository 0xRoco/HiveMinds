using HiveMinds.Models;
using HiveMinds.Services.Interfaces;
using HiveMinds.ViewModels;

namespace HiveMinds.Services;

public class AuthService : IAuthService
{
    private readonly ILogger<AuthService> _logger;
    private readonly ISecurityService _securityService;
    private readonly IAccountRepository _accountRepository;
    private readonly IVerificationService _verificationService;
    private readonly IAccountFactory _accountFactory;

    public AuthService(ILogger<AuthService> logger, IAccountRepository accountRepository,
        ISecurityService securityService, IVerificationService verificationService, IAccountFactory accountFactory)
    {
        _logger = logger;
        _accountRepository = accountRepository;
        _securityService = securityService;
        _verificationService = verificationService;
        _accountFactory = accountFactory;
    }

    public async Task<bool> Signup(SignupViewModel model, bool login = false)
    {
        if (_accountRepository.Exists(model.Username))
        {
            return false;
        }

        var account = await _accountFactory.CreateAccountModel(model);

        var databaseResult = await _accountRepository.CreateUser(account);
        if (databaseResult)
        {
            await _verificationService.SendEmailVerification(account.Username);
        }

        return databaseResult;
    }

    public async Task<bool> Login(LoginViewModel model)
    {
        var account = _accountRepository.GetByUsername(model.Username);
        if (account == null)
        {
            return false;
        }

        var salt = Convert.FromBase64String(account.PasswordSalt);
        var hash = Convert.FromBase64String(account.PasswordHash);
        var passwordVerified = await _securityService.VerifyPassword(model.Password, salt, hash);

        if (passwordVerified)
        {
            await ChangeLastLoginTime(account);
        }

        return passwordVerified;
    }

    public async Task<bool> Logout()
    {
        throw new NotImplementedException();
    }

    public async Task<bool> ResetPassword(string username, string token, string newPassword)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> SendPasswordReset(string username)
    {
        return false;
    }

    private async Task ChangeLastLoginTime(Account account)
    {
        account.LastLoginAt = DateTime.UtcNow;
        await _accountRepository.UpdateUser(account);
    }
}