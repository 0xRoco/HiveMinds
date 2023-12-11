using System.Net;
using System.Net.Mail;
using HiveMinds.API.Core;
using HiveMinds.API.Interfaces;
using HiveMinds.Models;
using Microsoft.Extensions.Options;

namespace HiveMinds.API.Services;

public class EmailService : IEmailService
{
    private readonly EmailConfig _emailConfig;

    public EmailService(IOptions<EmailConfig> settings)
    {
        _emailConfig = settings.Value;
    }
    
    public async Task<bool> SendEmailAsync(string to, string subject, string body)
    {
        var client = new SmtpClient(_emailConfig.SmtpServer)
        {
            Port = _emailConfig.SmtpPort,
            Credentials = new NetworkCredential(_emailConfig.Username, _emailConfig.Password),
            EnableSsl = _emailConfig.EnableSsl
        };
        
        var mailMessage = new MailMessage
        {
            From = new MailAddress("hiveminds@mdnite.com"),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };
        
        mailMessage.To.Add(to);

        await client.SendMailAsync(mailMessage);

        return true;
    }

    public async Task<string> PrepareEmailBody(string emailTemplatePath, Account account)
    {
        var emailTemplate = await File.ReadAllTextAsync(emailTemplatePath);
        var emailBody = emailTemplate
            .Replace("{Username}", account.Username)
            .Replace("{EmailCode}", account.EmailCode)
            .Replace("{Year}", DateTime.UtcNow.Year.ToString())
            .Replace("{BaseUrl}", "https://hiveminds.mdnite-vps.xyz");

        return emailBody;
    }
}