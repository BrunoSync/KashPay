using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KashPay.Application.Common.Interfaces.Infrastructure.Repositories;
using KashPay.Application.Common.OneOf;
using KashPay.Application.Common.OneOf.Errors;
using KashPay.Domain.Entities;
using KashPay.Domain.Enums;
using MediatR;
using OneOf;

namespace KashPay.Application.Features.Wallet.Commands.Deposit
{
    public class DepositHandler : IRequestHandler<DepositCommand, OneOf<DepositResponse, AppError>>
    {
        private readonly IWalletRepository _walletRepo;
        private readonly ITransactionRepository _transactionRepo;
        private readonly IUnitOfWork _uow;

        public DepositHandler(IWalletRepository walletRepo, ITransactionRepository transactionRepo, IUnitOfWork uow)
        {
            _walletRepo = walletRepo;
            _transactionRepo = transactionRepo;
            _uow = uow;
        }

        public async Task<OneOf<DepositResponse, AppError>> Handle(DepositCommand command, CancellationToken ct)
        {
            var wallet = await _walletRepo.FindWalletByUserIdAsync(command.userId, ct);

            if (wallet is null)
                return new WalletNotFoundError();

            wallet.Credit(command.Amount);

            var newTransaction = new Domain.Entities.Transaction(
                null,
                wallet.Id,
                command.Amount,
                TransactionType.Deposit
            );

            await _transactionRepo.Add(newTransaction);
            await _uow.CommitAsync(ct);

            return new DepositResponse(
                newTransaction.Id,
                newTransaction.Amount,
                wallet.Balance,
                DateTime.UtcNow
            );
        }
    }
}