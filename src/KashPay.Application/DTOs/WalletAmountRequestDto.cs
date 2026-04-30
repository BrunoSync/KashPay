using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KashPay.Application.DTOs
{
    public record WalletAmountRequestDto
    (
        decimal Amount
    );
}