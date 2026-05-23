using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using KashPay.Application.Common.Interfaces.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace KashPay.Infrastructure.Common.Services
{
    public class EmailService :IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendPasswordResetEmailAsync(string toEmail, string code, CancellationToken ct)
        {
            var fromEmail = _configuration["SMTP_FROM"];
            var host = _configuration["SMTP_HOST"];
            var port = _configuration["SMTP_PORT"];

            var smtpClient = new SmtpClient(host, int.Parse(port!));

            var subject = "KashPay - Password Reset Request";
            var body = $"Your KashPay password reset code is: {code}\n\nThis code expires in 30 minutes. If you didn't request this, ignore this email.";
            var mailMessage = new MailMessage(
                fromEmail!,
                toEmail,
                subject,
                body
            );

            await smtpClient.SendMailAsync(mailMessage, ct);
        }
    }
}