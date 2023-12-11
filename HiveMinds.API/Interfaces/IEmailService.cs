using HiveMinds.Models;

namespace HiveMinds.API.Interfaces;

public interface IEmailService
{
    Task<bool> SendEmailAsync(string to, string subject, string body);
    Task<string> PrepareEmailBody(string emailTemplatePath, Account account); 
}