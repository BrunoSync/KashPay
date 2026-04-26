using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KashPay.Application.Common.OneOf;
using MediatR;
using OneOf;

namespace KashPay.Application.Features.Auth.Login.Register.Commands
{
    public record RegisterCommand
    (
        string FirstName,
        string LastName,
        string Email,
        string Cpf,
        string Password
    ) : IRequest<OneOf<RegisterResponse, AppError>>;
}