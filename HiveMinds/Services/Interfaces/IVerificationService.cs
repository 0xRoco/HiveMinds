using HiveMinds.Models;

namespace HiveMinds.Services.Interfaces;

public interface IVerificationService
{
    Task<bool> RequestVerification(string username, string reason);
    Task<bool> SetVerificationStatus(int id, int status);
    Task<bool> VerifyUser(string username);
    Task<bool> VerifyEmail(string username, string code);
    Task<bool> VerifyPhoneNumber(string username, string code);
    Task<bool> SendEmailVerification(string username);
    Task<bool> SendPhoneVerification(string username);
}