using System.Security.Cryptography;
using HiveMinds.DataTypes;
using HiveMinds.Models;
using HiveMinds.Services.Interfaces;

namespace HiveMinds.Services;

public class VerificationService : IVerificationService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IEmailService _emailService;

    public VerificationService(IAccountRepository accountRepository, IEmailService emailService)
    {
        _accountRepository = accountRepository;
        _emailService = emailService;
    }


    public async Task<bool> RequestVerification(string username, string reason)
    {
        var account = await _accountRepository.GetByUsername(username);
        if (account == null)
        {
            return false;
        }
        
        // TODO: Check if user already has a pending request
        var existingRequest = await _accountRepository.GetVerificationRequestsByUserId(account.Id);
        if (existingRequest is { Status: VerificationStatus.Pending }) return false;
        
        var request = new VerificationRequest
        {
            Id = RandomNumberGenerator.GetInt32(100000000,
                999999999),
            UserId = account.Id,
            Reason = reason,
            Status = VerificationStatus.Pending,
            Date = DateTime.UtcNow
        };

        var databaseResult = await _accountRepository.CreateVerificationRequest(request);
        return databaseResult;
    }
    
    public async Task<bool> SetVerificationStatus(int id, int status)
    {
        var request = await _accountRepository.GetVerificationRequestById(id);
        
        if (request == null)
        {
            return false;
        }

        request.Status = (VerificationStatus) status;
        
        if (request.Status == VerificationStatus.Approved)
        {
            var account = await _accountRepository.GetById(request.UserId);
            if (account == null)
            {
                return false;
            }

            account.IsVerified = true;
            await _accountRepository.UpdateUser(account);
        }
        
        var databaseResult = await _accountRepository.UpdateVerificationRequest(request);
        return databaseResult;
    }

    public async Task<bool> VerifyUser(string username)
    {
        var account = await _accountRepository.GetByUsername(username);
        if (account == null)
        {
            return false;
        }
        
        account.IsVerified = true;
        var databaseResult = await _accountRepository.UpdateUser(account);
        
        return databaseResult;
    }

    public async Task<bool> VerifyEmail(string username, string code)
    {
        var account = await _accountRepository.GetByUsername(username);
        if (account == null || account.EmailCode != code)
        {
            return false;
        }

        account.IsEmailVerified = true;
        account.EmailCode = $"INVALID-{Guid.NewGuid()}";
        var databaseResult = await _accountRepository.UpdateUser(account);
            
        return databaseResult;
    }

    public async Task<bool> VerifyPhoneNumber(string username, string code)
    {
        var account = await _accountRepository.GetByUsername(username);
        if (account == null || account.PhoneNumberCode != code)
        {
            return false;
        }

        account.IsPhoneNumberVerified = true;
        account.PhoneNumberCode = "";
        var databaseResult = await _accountRepository.UpdateUser(account);

        return databaseResult;
    }

    public async Task<bool> SendEmailVerification(string username)
    {
        var account = await _accountRepository.GetByUsername(username);
        if (account == null) return false;

        const string subject = "Verify Your HiveMinds Account - Action Required";
        var body = $@"Dear {account.Username},
Welcome to ""HiveMinds"", the platform where your voice becomes one with the Party's collective wisdom. To ensure the integrity of our community and the accuracy of your allegiance, we kindly request that you complete your account registration by verifying your email address.

Please follow these steps to verify your email address:

1. Click on the Verification Link: $GetBaseUrl/Profile/verify-email?verificationCode={account.EmailCode}

2. If the link does not work, you can also manually verify your email by using the following verification code: {account.EmailCode}

Remember, by confirming your email address, you actively participate in the Party's cause and demonstrate your dedication to maintaining our collective consciousness. Together, we amplify our voices and serve the greater good.

Thank you for your compliance, and welcome to the Party.

Sincerely,
The ""HiveMinds"" Team";

        var result = await _emailService.SendEmailAsync(account.Email, subject, body);
        return result;
    }

    public async Task<bool> SendPhoneVerification(string username)
    {
        throw new NotImplementedException();
    }
}