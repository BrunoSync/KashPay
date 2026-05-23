using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KashPay.Application.Common.Interfaces.Infrastructure
{
    public interface IEmailService
    {
        Task SendPasswordResetEmailAsync(string toEmail, string code, CancellationToken ct);
    }
}