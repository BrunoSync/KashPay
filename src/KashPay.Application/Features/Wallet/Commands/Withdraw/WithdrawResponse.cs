using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KashPay.Application.Features.Wallet.Commands.Withdraw
{
    public record WithdrawResponse
    (
        Guid TransactionId,
        decimal Amount,
        decimal NewBalance,
        DateTime CreatedAt
    );
}