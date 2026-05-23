using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KashPay.Application.Features.Auth.ForgetPassword.Commands
{
    public sealed record ResetPasswordResponse
    (
        string Message
    );
}