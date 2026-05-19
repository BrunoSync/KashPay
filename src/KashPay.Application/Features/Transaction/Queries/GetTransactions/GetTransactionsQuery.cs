using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KashPay.Application.Common.OneOf;
using MediatR;
using OneOf;

namespace KashPay.Application.Features.Transaction.Queries.GetTransactions
{
    public record GetTransactionsQuery
    (
        Guid UserId,
        int PageSize,
        (DateTime?, Guid) Cursor
    ) : IRequest<OneOf<GetTransactionsResponse, AppError>>;
}