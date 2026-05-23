using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KashPay.Application.Features.Auth.ForgotPassword.Commands
{
    public sealed record ForgotPasswordResponse
    (
        string Message
    );
}