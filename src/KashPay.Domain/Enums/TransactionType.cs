using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KashPay.Domain.Enums
{
    public enum TransactionType
    {
        Deposit = 1,
        WithDraw = 2,
        TransferP2P = 3
    }
}