using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace AmazonClone.Infrastructure.Authentication
{
    public class EmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration config, ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendVerificationEmail(string email, string token)
        {
            // URL encode token and email so special characters (like +, /, =) are preserved
            var verifyUrl = $"{_config["App:BaseUrl"]}/verify-email?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(email)}";

            _logger.LogInformation("=========================================================");
            _logger.LogInformation("VERIFICATION EMAIL FOR {Email}:", email);
            _logger.LogInformation("Link: {VerifyUrl}", verifyUrl);
            _logger.LogInformation("=========================================================");

            try
            {
                var message = new MailMessage();
                var fromEmail = _config["Email:From"] ?? "noreply@amazonclone.com";
                message.From = new MailAddress(fromEmail);
                message.To.Add(email);
                message.Subject = "Verify Your Email - Amazon Clone";
                message.Body = $"Please verify your email by clicking the following link:\n\n{verifyUrl}";

                var smtpServer = _config["Email:SmtpServer"];
                if (string.IsNullOrEmpty(smtpServer) || smtpServer == "string" || smtpServer == "smtp.gmail.com")
                {
                    // If credentials are placeholder, log and return without failing registration
                    if (_config["Email:Username"]?.Contains("example") == true || _config["Email:Username"]?.Contains("your-email") == true)
                    {
                        _logger.LogWarning("SMTP credentials in appsettings.json appear to be placeholders. Email was logged above but not sent via SMTP.");
                        return;
                    }
                }

                using var smtp = new SmtpClient(smtpServer)
                {
                    Port = int.Parse(_config["Email:Port"] ?? "587"),
                    Credentials = new NetworkCredential(
                        _config["Email:Username"],
                        _config["Email:Password"]
                    ),
                    EnableSsl = true
                };

                await smtp.SendMailAsync(message);
                _logger.LogInformation("Verification email sent successfully via SMTP to {Email}.", email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send verification email via SMTP to {Email}. However, the verification link was logged above.", email);
            }
        }

        public async Task SendPasswordResetEmail(string email, string token)
        {
            var resetUrl = $"{_config["App:BaseUrl"]}/reset-password?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(email)}";

            _logger.LogInformation("=========================================================");
            _logger.LogInformation("PASSWORD RESET EMAIL FOR {Email}:", email);
            _logger.LogInformation("Link: {ResetUrl}", resetUrl);
            _logger.LogInformation("=========================================================");

            try
            {
                var message = new MailMessage();
                var fromEmail = _config["Email:From"] ?? "noreply@amazonclone.com";
                message.From = new MailAddress(fromEmail);
                message.To.Add(email);
                message.Subject = "Reset Your Password - Amazon Clone";
                message.Body = $"Please reset your password by clicking the following link:\n\n{resetUrl}";

                var smtpServer = _config["Email:SmtpServer"];
                if (string.IsNullOrEmpty(smtpServer) || smtpServer == "string" || smtpServer == "smtp.gmail.com")
                {
                    if (_config["Email:Username"]?.Contains("example") == true || _config["Email:Username"]?.Contains("your-email") == true)
                    {
                        _logger.LogWarning("SMTP credentials in appsettings.json appear to be placeholders. Email was logged above but not sent via SMTP.");
                        return;
                    }
                }

                using var smtp = new SmtpClient(smtpServer)
                {
                    Port = int.Parse(_config["Email:Port"] ?? "587"),
                    Credentials = new NetworkCredential(
                        _config["Email:Username"],
                        _config["Email:Password"]
                    ),
                    EnableSsl = true
                };

                await smtp.SendMailAsync(message);
                _logger.LogInformation("Password reset email sent successfully via SMTP to {Email}.", email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send password reset email via SMTP to {Email}. However, the reset link was logged above.", email);
            }
        }
    }
}

