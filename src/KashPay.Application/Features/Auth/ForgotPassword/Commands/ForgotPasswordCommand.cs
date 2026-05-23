using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KashPay.Application.Common.OneOf;
using MediatR;
using OneOf;

namespace KashPay.Application.Features.Auth.ForgotPassword.Commands
{
    public record ForgotPasswordCommand
    (
        string Email
    ) : IRequest<ForgotPasswordResponse>;
}