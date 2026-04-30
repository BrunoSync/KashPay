using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KashPay.Application.Common.Interfaces.Infrastructure.Repositories;
using KashPay.Application.Common.OneOf;
using KashPay.Application.Common.OneOf.Errors;
using KashPay.Application.DTOs;
using MediatR;
using OneOf;

namespace KashPay.Application.Features.Transaction.Queries.GetTransactions
{
    public class GetTransactionsHandler : IRequestHandler<GetTransactionsQuery, OneOf<GetTransactionsResponse, AppError>>
    {
        private readonly IWalletRepository _walletRepo;
        private readonly ITransactionRepository _transactionRepo;
        private readonly IUnitOfWork _uow;

        public GetTransactionsHandler(IWalletRepository walletRepo, ITransactionRepository transactionRepo, IUnitOfWork uow)
        {
            _walletRepo = walletRepo;
            _transactionRepo = transactionRepo;
            _uow = uow;
        }

        public async Task<OneOf<GetTransactionsResponse, AppError>> Handle(GetTransactionsQuery query, CancellationToken ct)
        {
            var wallet = await _walletRepo.FindWalletByUserIdAsync(query.UserId, ct);

            if (wallet is null)
                return new WalletNotFoundError();

            var transactions = await _transactionRepo.GetByWalletIdAsync(
                wallet.Id,
                query.PageSize,
                query.Cursor,
                ct
            );

            var transactionDtos = transactions.Select(t => new TransactionDto(
                t.FromAccountId,
                t.ToAccountId,
                t.Amount,
                t.Type,
                t.CreatedAt
            )).ToList();

            var nextCursor = transactions.Count == query.PageSize
                ? transactions.Last().CreatedAt
                : null as DateTime?;

            return new GetTransactionsResponse(transactionDtos, nextCursor);
        }
    }
}