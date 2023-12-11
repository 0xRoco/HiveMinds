using HiveMinds.API.Core;
using HiveMinds.API.Interfaces;
using HiveMinds.Common;
using HiveMinds.DTO;
using HiveMinds.Models;
using Microsoft.Extensions.Options;

namespace HiveMinds.API.Services;

public class AccountFactory : IAccountFactory
{
    private readonly ISecurityService _securityService;
    private readonly HiveMindsConfig _config;

    public AccountFactory(ISecurityService securityService, IOptions<HiveMindsConfig> config)
    {
        _securityService = securityService;
        _config = config.Value;
    }

    public Account CreateAccountModel(SignupDto model)
    {
        var hash = _securityService.CreatePasswordHash(model.Password);
        var accountModel = new Account
        {
            Id = _securityService.GenerateId(min: 100000000,
                max: 999999999),
            Username = model.Username,
            Email = model.Email,
            PasswordHash = hash,
            PhoneNumber = model.PhoneNumber,
            EmailCode = _securityService.GenerateCode(),
            PhoneNumberCode = _securityService.GenerateCode(),
            PasswordResetToken = "",
            ProfilePictureUrl = _config.DefaultProfileImage,
            LoyaltyStatement = model.PartyLoyaltyStatement,
            Bio = model.Bio,
            IsEmailVerified = false,
            IsPhoneNumberVerified = false,
            IsVerified = false,
            Status = AccountStatus.Active,
            Role = AccountRole.User,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            DeletedAt = default,
            LastLoginAt = DateTime.UtcNow,
            LastLoginIp = "",
        };

        return accountModel;
    }

    public Account CreateAccountModel(string username, string password)
    {
        throw new NotImplementedException();
    }

    public Account CreateDemoAccountModel()
    {
        throw new NotImplementedException();
    }
}