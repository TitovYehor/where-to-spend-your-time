namespace WhereToSpendYourTime.Api.Services.Email;

/// <summary>
/// Provides email sending functionality using SMTP
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends an HTML email asynchronously
    /// </summary>
    /// <param name="to">Recipient email address</param>
    /// <param name="subject">Email subject line</param>
    /// <param name="htmlContent">HTML content of the email</param>
    Task SendEmailAsync(string to, string subject, string htmlContent);
}