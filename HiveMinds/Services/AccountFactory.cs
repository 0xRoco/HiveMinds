using HiveMinds.Models;
using HiveMinds.Services.Interfaces;
using HiveMinds.ViewModels;

namespace HiveMinds.Services;

public class AccountFactory : IAccountFactory
{
    private readonly ISecurityService _securityService;

    public AccountFactory(ISecurityService securityService)
    {
        _securityService = securityService;
    }

    public async Task<Account> CreateAccountModel(SignupViewModel model)
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

    public async Task<Account> CreateAccountModel(string username, string password)
    {
        throw new NotImplementedException();
    }

    public async Task<Account> CreateDemoAccountModel()
    {
        throw new NotImplementedException();
    }
}