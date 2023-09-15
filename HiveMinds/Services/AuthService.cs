using HiveMinds.API.Interfaces;
using HiveMinds.Models;
using HiveMinds.Services.Interfaces;
using HiveMinds.ViewModels;
using Microsoft.Extensions.Logging;

namespace HiveMinds.Services;

public class AuthService : IAuthService
{
    private readonly ILogger<AuthService> _logger;
    private readonly ISecurityService _securityService;
    private readonly IAccountRepository _accountRepository;
    private readonly IVerificationService _verificationService;

    public AuthService(ILogger<AuthService> logger, IAccountRepository accountRepository,
        ISecurityService securityService, IVerificationService verificationService)
    {
        _logger = logger;
        _accountRepository = accountRepository;
        _securityService = securityService;
        _verificationService = verificationService;
    }

    public async Task<bool> Signup(SignupViewModel model, bool login = false)
    {
        try
        {
            if (_accountRepository.Exists(model.Username))
            {
                return false;
            }

            var account = await CreateAccountModel(model);
            if (account == null)
            {
                return false;
            }

            var databaseResult = await _accountRepository.CreateUser(account);
            if (databaseResult)
            {
               await _verificationService.SendEmailVerification(account.Username);
            }
            
            return databaseResult;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error creating account");
            return false;
        }
    }

    public async Task<bool> Login(LoginViewModel model)
    {
        try
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
        catch (Exception e)
        {
            _logger.LogError(e, $"Error logging in user {model.Username}");
            return false;
        }
    }

    // TODO: Implement LogoutAsync
    public Task<bool> Logout()
    {
        try
        {
            throw new NotImplementedException();
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error logging out user");
            return Task.FromResult(false);
        }
    }

    public Task<bool> ResetPassword(string username, string token, string newPassword)
    {
        try
        {
            throw new NotImplementedException();
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error resetting password for user {username}");
            return Task.FromResult(false);
        }
    }

    public Task<bool> SendPasswordReset(string username)
    {
        try
        {
            throw new NotImplementedException();
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Error forgot password for user {username}");
            return Task.FromResult(false);
        }
    }

    private async Task<Account?> CreateAccountModel(SignupViewModel model)
    {
        var salt = _securityService.CreateSalt();
        var hash = Convert.ToBase64String(await _securityService.CreateHash(model.Password, salt));
        var accountModel = new Account
        {
            Id = _securityService.GenerateId(min: 100000000, max: 999999999),
            Username = model.Username,
            Email = model.Email,
            PasswordHash = hash,
            PasswordSalt = Convert.ToBase64String(salt),
            PhoneNumber = model.PhoneNumber,
            EmailCode = _securityService.GenerateCode(),
            PhoneNumberCode = _securityService.GenerateCode(),
            LoginToken = "",
            PasswordResetToken = "",
            ProfilePictureUrl = "",
            LoyaltyStatement = model.PartyLoyaltyStatement,
            Bio = model.Bio,
            VerificationRequest = "",
            IsEmailVerified = false,
            IsPhoneNumberVerified = false,
            IsVerified = false,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            LastLoginAt = DateTime.UtcNow,
            LastLoginIp = "",
        };

        return accountModel;
    }

    private async Task ChangeLastLoginTime(Account account)
    {
        account.LastLoginAt = DateTime.UtcNow;
        await _accountRepository.UpdateUser(account);
    }
}