using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KashPay.Application.Common.OneOf;
using MediatR;
using OneOf;

namespace KashPay.Application.Features.Auth.Login.Queries
{
    public record LoginCommand
    (
        string Credentials,
        string Password
    ) : IRequest<OneOf<LoginResponse, AppError>>;
}