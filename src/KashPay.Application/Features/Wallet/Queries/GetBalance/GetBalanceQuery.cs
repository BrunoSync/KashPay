using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KashPay.Application.Common.OneOf;
using MediatR;
using OneOf;

namespace KashPay.Application.Features.Wallet.Queries.GetBalance
{
    public record GetBalanceQuery
    (
        Guid UserId
    ) : IRequest<OneOf<GetBalanceResponse, AppError>>;
}