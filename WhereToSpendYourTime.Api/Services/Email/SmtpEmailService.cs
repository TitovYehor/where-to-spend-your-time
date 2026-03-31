using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Options;
using WhereToSpendYourTime.Api.Services.Email.Config;

namespace WhereToSpendYourTime.Api.Services.Email;

/// <summary>
/// Implements email sending using SMTP.
/// Supports SSL and HTML email messages
/// </summary>
public class SmtpEmailService : IEmailService
{
    private readonly EmailSettings _settings;

    public SmtpEmailService(IOptions<EmailSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task SendEmailAsync(string to, string subject, string htmlContent)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
        email.To.Add(new MailboxAddress(to, to));
        email.Subject = subject;
        email.Body = new TextPart("html") { Text = htmlContent };

        using var smtp = new MailKit.Net.Smtp.SmtpClient();

        try
        {
            await smtp.ConnectAsync(_settings.SmtpHost, _settings.SmtpPort, SecureSocketOptions.SslOnConnect);

            await smtp.AuthenticateAsync(_settings.Username, _settings.Password);

            await smtp.SendAsync(email);

            await smtp.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Email sending failed: {ex.Message}", ex);
        }
    }
}