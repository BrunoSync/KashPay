using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KashPay.Application.Common.OneOf;
using MediatR;
using OneOf;

namespace KashPay.Application.Features.Auth.Refresh.Commands
{
    public record RefreshCommand
    (
        string RefreshToken
    ) : IRequest<OneOf<RefreshResponse, AppError>>;
}