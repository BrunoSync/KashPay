using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KashPay.Application.Common.OneOf;
using MediatR;
using OneOf;

namespace KashPay.Application.Features.Wallet.Commands.Transfer
{
    public record TransferCommand
    (
        Guid UserId,
        string AccountNumber,
        decimal Amount
    ) : IRequest<OneOf<TransferResponse, AppError>>;
}