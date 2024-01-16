using HiveMinds.API.Core;
using HiveMinds.API.Interfaces;
using HiveMinds.Common;
using HiveMinds.DTO;
using Microsoft.Extensions.Options;

namespace HiveMinds.API.Services;

public class AuthService : IAuthService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IEmailService _emailService;
    private readonly IAccountFactory _accountFactory;
    private readonly ISecurityService _securityService;
    private readonly HiveMindsConfig _hiveMindsConfig;

    private readonly ILogger<IAuthService> _logger;

    public enum AuthResults
    {
        Success,
        UsernameOrPasswordIncorrect,
        AccountDeactivatedOrSuspended,
        UsernameAlreadyExists,
        InternalError
    }


    public AuthService(IAccountRepository accountRepository, IEmailService emailService, IAccountFactory accountFactory,
        ISecurityService securityService, IOptions<HiveMindsConfig> hiveMindsConfig, ILogger<IAuthService> logger)
    {
        _accountRepository = accountRepository;
        _emailService = emailService;
        _accountFactory = accountFactory;
        _securityService = securityService;
        _logger = logger;
        _hiveMindsConfig = hiveMindsConfig.Value;
    }

    public async Task<Result<LoginResponseDto, AuthResults>> Login(LoginDto loginDto)
    {
        _logger.LogInformation("Attempting to login user: {username}", loginDto.Username);
        var account = await _accountRepository.GetByUsername(loginDto.Username);
        if (account is null || !_securityService.VerifyPasswordHash(loginDto.Password, account.PasswordHash))
        {
            _logger.LogInformation("Failed login attempt (Invalid username/password) for user: {username}",
                loginDto.Username);
            return Result<LoginResponseDto, AuthResults>.Failure("Invalid username or password",
                AuthResults.UsernameOrPasswordIncorrect);
        }

        _logger.LogInformation("Successfully retrieved account for user: {username}", account.Username);

        if (account.Status != AccountStatus.Active)
        {
            return Result<LoginResponseDto, AuthResults>.Failure("Account is deactivated or suspended",
                AuthResults.AccountDeactivatedOrSuspended);
        }

        var loginResponse = new LoginResponseDto
        {
            AccountId = account.Id,
            Token = _securityService.GenerateToken(account),
            Expiration = DateTime.UtcNow.AddHours(_hiveMindsConfig.TokenExpirationHours)
        };

        await _accountRepository.UpdateLastLogin(account);
        _logger.LogInformation("Updated last login for account: {username}", account.Username);

        return Result<LoginResponseDto, AuthResults>.Success(loginResponse);
    }

    public async Task<Result<int, AuthResults>> Signup(SignupDto signupDto)
    {
        _logger.LogInformation("Attempting to signup user: {username}", signupDto.Username);

        var accountCheck = await _accountRepository.GetByUsername(signupDto.Username);
        if (accountCheck is not null)
        {
            _logger.LogInformation("Sign up attempt failed, Username is already taken: {username}", signupDto.Username);
            return Result<int, AuthResults>.Failure("An account with the specified username already exists",
                AuthResults.UsernameAlreadyExists);
        }

        _logger.LogInformation("Username is available: {username}", signupDto.Username);

        var account = _accountFactory.CreateAccountModel(signupDto);

        var databaseResult = await _accountRepository.CreateUser(account);

        if (!databaseResult)
        {
            _logger.LogError("An error occurred while signing up user: {username}", signupDto.Username);
            return Result<int, AuthResults>.Failure("An error occurred while signing up",
                AuthResults.InternalError);
        }

        var emailBody = await _emailService.PrepareEmailBody("Data/emailTemplate.html", account);

        await _emailService.SendEmailAsync(signupDto.Email, "Verify Your HiveMinds Account", emailBody);

        _logger.LogInformation("Successfully signed up user: {username}", signupDto.Username);

        return Result<int, AuthResults>.Success(account.Id);
    }
}