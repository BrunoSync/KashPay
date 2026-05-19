using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KashPay.Application.DTOs;
using MediatR;

namespace KashPay.Application.Features.Transaction.Queries.GetTransactions
{
    public record GetTransactionsResponse
    (
        List<TransactionDto> Transactions,
        (DateTime?, Guid) Cursor
    );
}