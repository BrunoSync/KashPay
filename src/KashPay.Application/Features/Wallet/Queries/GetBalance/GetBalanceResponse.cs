using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KashPay.Application.Features.Wallet.Queries.GetBalance
{
    public record GetBalanceResponse
    (
        decimal Balance
    );
}