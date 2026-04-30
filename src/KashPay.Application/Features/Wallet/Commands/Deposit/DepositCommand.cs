using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KashPay.Application.Common.OneOf;
using MediatR;
using OneOf;

namespace KashPay.Application.Features.Wallet.Commands.Deposit
{
    public record DepositCommand
    (
        Guid userId,
        decimal Amount
    ) : IRequest<OneOf<DepositResponse, AppError>>;
}