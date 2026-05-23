using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KashPay.Application.Common.OneOf;
using MediatR;
using OneOf;

namespace KashPay.Application.Features.Auth.ForgetPassword.Commands
{
    public record ResetPasswordCommand
    (
        string Email,
        string Token,
        string NewPassword,
        string ConfirmNewPassword
    ) : IRequest<OneOf<ResetPasswordResponse, AppError>>;
}