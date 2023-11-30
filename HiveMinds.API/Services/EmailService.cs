using System.Net;
using System.Net.Mail;
using HiveMinds.API.Common;
using HiveMinds.API.Services.Interfaces;
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
            From = new MailAddress(""),
            Subject = subject,
            Body = body,
            Priority = MailPriority.High,
        };
        
        mailMessage.To.Add(to);

        await client.SendMailAsync(mailMessage);

        return true;
    }
}