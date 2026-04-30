using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KashPay.Domain.Enums;

namespace KashPay.Application.DTOs
{
    public sealed record TransactionDto
    (
        Guid? FromAccountId,
        Guid? ToAccountId,
        decimal Amount,
        TransactionType Type,
        DateTime CreatedAt
    );
}