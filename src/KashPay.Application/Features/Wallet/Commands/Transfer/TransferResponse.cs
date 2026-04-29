using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KashPay.Application.Features.Wallet.Commands.Transfer
{
    public record TransferResponse
    (
        string FromAccount,
        string ToAccount,
        decimal Amount,
        DateTime CreatedAt
    );
}