using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FurniFlowUz.Infrastructure.Services;

public interface IEmailService
{
    /// <summary>
    /// Sends an email to a single recipient
    /// </summary>
    Task SendEmailAsync(string to, string subject, string body, bool isHtml = true);

    /// <summary>
    /// Sends an email to multiple recipients
    /// </summary>
    Task SendBulkEmailAsync(IEnumerable<string> recipients, string subject, string body, bool isHtml = true);

    /// <summary>
    /// Sends an email with attachments
    /// </summary>
    Task SendEmailWithAttachmentAsync(string to, string subject, string body, IEnumerable<string> attachmentPaths, bool isHtml = true);
}

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;
    private readonly string _smtpServer;
    private readonly int _smtpPort;
    private readonly string _fromEmail;
    private readonly string _fromName;
    private readonly string _username;
    private readonly string _password;
    private readonly bool _enableSsl;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;

        // Read SMTP configuration from appsettings.json
        _smtpServer = _configuration["EmailSettings:SmtpServer"] ?? "smtp.gmail.com";
        _smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
        _fromEmail = _configuration["EmailSettings:FromEmail"] ?? "noreply@furniflow.uz";
        _fromName = _configuration["EmailSettings:FromName"] ?? "FurniFlow Uz";
        _username = _configuration["EmailSettings:Username"] ?? "";
        _password = _configuration["EmailSettings:Password"] ?? "";
        _enableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"] ?? "true");
    }

    public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
    {
        if (string.IsNullOrWhiteSpace(to))
        {
            throw new ArgumentException("Recipient email address is required.", nameof(to));
        }

        if (string.IsNullOrWhiteSpace(subject))
        {
            throw new ArgumentException("Email subject is required.", nameof(subject));
        }

        try
        {
            using var mailMessage = new MailMessage
            {
                From = new MailAddress(_fromEmail, _fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };

            mailMessage.To.Add(to);

            await SendMailAsync(mailMessage);

            _logger.LogInformation("Email sent successfully to {Recipient}", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Recipient}", to);
            throw new InvalidOperationException($"Failed to send email to {to}. See inner exception for details.", ex);
        }
    }

    public async Task SendBulkEmailAsync(IEnumerable<string> recipients, string subject, string body, bool isHtml = true)
    {
        if (recipients == null || !recipients.Any())
        {
            throw new ArgumentException("At least one recipient is required.", nameof(recipients));
        }

        if (string.IsNullOrWhiteSpace(subject))
        {
            throw new ArgumentException("Email subject is required.", nameof(subject));
        }

        var failedRecipients = new List<string>();

        foreach (var recipient in recipients)
        {
            try
            {
                await SendEmailAsync(recipient, subject, body, isHtml);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Recipient}", recipient);
                failedRecipients.Add(recipient);
            }
        }

        if (failedRecipients.Any())
        {
            _logger.LogWarning("Failed to send emails to {Count} recipient(s): {Recipients}",
                failedRecipients.Count, string.Join(", ", failedRecipients));
        }
        else
        {
            _logger.LogInformation("Bulk email sent successfully to {Count} recipient(s)", recipients.Count());
        }
    }

    public async Task SendEmailWithAttachmentAsync(string to, string subject, string body, IEnumerable<string> attachmentPaths, bool isHtml = true)
    {
        if (string.IsNullOrWhiteSpace(to))
        {
            throw new ArgumentException("Recipient email address is required.", nameof(to));
        }

        if (string.IsNullOrWhiteSpace(subject))
        {
            throw new ArgumentException("Email subject is required.", nameof(subject));
        }

        if (attachmentPaths == null || !attachmentPaths.Any())
        {
            throw new ArgumentException("At least one attachment is required.", nameof(attachmentPaths));
        }

        try
        {
            using var mailMessage = new MailMessage
            {
                From = new MailAddress(_fromEmail, _fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };

            mailMessage.To.Add(to);

            // Add attachments
            foreach (var attachmentPath in attachmentPaths)
            {
                if (File.Exists(attachmentPath))
                {
                    var attachment = new Attachment(attachmentPath);
                    mailMessage.Attachments.Add(attachment);
                }
                else
                {
                    _logger.LogWarning("Attachment file not found: {Path}", attachmentPath);
                }
            }

            await SendMailAsync(mailMessage);

            _logger.LogInformation("Email with {Count} attachment(s) sent successfully to {Recipient}",
                mailMessage.Attachments.Count, to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email with attachments to {Recipient}", to);
            throw new InvalidOperationException($"Failed to send email with attachments to {to}. See inner exception for details.", ex);
        }
    }

    private async Task SendMailAsync(MailMessage mailMessage)
    {
        // Check if SMTP credentials are configured
        if (string.IsNullOrWhiteSpace(_username) || string.IsNullOrWhiteSpace(_password))
        {
            _logger.LogWarning("SMTP credentials not configured. Email will not be sent. Configure EmailSettings in appsettings.json");
            _logger.LogInformation("Email details - To: {To}, Subject: {Subject}",
                string.Join(", ", mailMessage.To.Select(x => x.Address)), mailMessage.Subject);
            return;
        }

        using var smtpClient = new SmtpClient(_smtpServer, _smtpPort)
        {
            EnableSsl = _enableSsl,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(_username, _password)
        };

        await smtpClient.SendMailAsync(mailMessage);
    }
}

/// <summary>
/// Email templates for common notifications
/// </summary>
public static class EmailTemplates
{
    public static string GetContractCreatedTemplate(string contractNumber, string customerName, decimal totalAmount, DateTime deadline)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #1e40af; color: white; padding: 20px; text-align: center; }}
        .content {{ background-color: #f9fafb; padding: 20px; }}
        .info-box {{ background-color: white; padding: 15px; margin: 10px 0; border-left: 4px solid #1e40af; }}
        .footer {{ text-align: center; padding: 20px; font-size: 12px; color: #666; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>New Contract Created</h1>
        </div>
        <div class='content'>
            <p>Dear {customerName},</p>
            <p>A new contract has been created for your order.</p>
            <div class='info-box'>
                <p><strong>Contract Number:</strong> {contractNumber}</p>
                <p><strong>Total Amount:</strong> {totalAmount:N2} UZS</p>
                <p><strong>Deadline:</strong> {deadline:dd MMMM yyyy}</p>
            </div>
            <p>Please review the contract details and contact us if you have any questions.</p>
        </div>
        <div class='footer'>
            <p>&copy; 2024 FurniFlow Uz. All rights reserved.</p>
            <p>Tashkent, Uzbekistan | Phone: +998 (71) 123-45-67</p>
        </div>
    </div>
</body>
</html>";
    }

    public static string GetOrderStatusUpdateTemplate(string orderNumber, string customerName, string status, decimal progressPercentage)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #1e40af; color: white; padding: 20px; text-align: center; }}
        .content {{ background-color: #f9fafb; padding: 20px; }}
        .info-box {{ background-color: white; padding: 15px; margin: 10px 0; border-left: 4px solid #1e40af; }}
        .progress-bar {{ background-color: #e5e7eb; height: 30px; border-radius: 15px; overflow: hidden; }}
        .progress-fill {{ background-color: #10b981; height: 100%; line-height: 30px; color: white; text-align: center; font-weight: bold; }}
        .footer {{ text-align: center; padding: 20px; font-size: 12px; color: #666; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Order Status Update</h1>
        </div>
        <div class='content'>
            <p>Dear {customerName},</p>
            <p>Your order status has been updated.</p>
            <div class='info-box'>
                <p><strong>Order Number:</strong> {orderNumber}</p>
                <p><strong>Current Status:</strong> {status}</p>
                <p><strong>Progress:</strong></p>
                <div class='progress-bar'>
                    <div class='progress-fill' style='width: {progressPercentage}%'>{progressPercentage}%</div>
                </div>
            </div>
            <p>Thank you for choosing FurniFlow Uz!</p>
        </div>
        <div class='footer'>
            <p>&copy; 2024 FurniFlow Uz. All rights reserved.</p>
            <p>Tashkent, Uzbekistan | Phone: +998 (71) 123-45-67</p>
        </div>
    </div>
</body>
</html>";
    }

    public static string GetTaskAssignmentTemplate(string taskName, string assigneeName, DateTime deadline, string orderNumber)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #1e40af; color: white; padding: 20px; text-align: center; }}
        .content {{ background-color: #f9fafb; padding: 20px; }}
        .info-box {{ background-color: white; padding: 15px; margin: 10px 0; border-left: 4px solid #1e40af; }}
        .footer {{ text-align: center; padding: 20px; font-size: 12px; color: #666; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>New Task Assignment</h1>
        </div>
        <div class='content'>
            <p>Dear {assigneeName},</p>
            <p>You have been assigned a new task.</p>
            <div class='info-box'>
                <p><strong>Task:</strong> {taskName}</p>
                <p><strong>Order Number:</strong> {orderNumber}</p>
                <p><strong>Deadline:</strong> {deadline:dd MMMM yyyy}</p>
            </div>
            <p>Please review the task details in the system and start working on it.</p>
        </div>
        <div class='footer'>
            <p>&copy; 2024 FurniFlow Uz. All rights reserved.</p>
            <p>Tashkent, Uzbekistan | Phone: +998 (71) 123-45-67</p>
        </div>
    </div>
</body>
</html>";
    }
}
