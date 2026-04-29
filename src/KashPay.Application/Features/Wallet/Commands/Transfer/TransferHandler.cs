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

namespace KashPay.Application.Features.Wallet.Commands.Transfer
{
    public class TransferHandler : IRequestHandler<TransferCommand, OneOf<TransferResponse, AppError>>
    {
        private readonly IWalletRepository _walletRepo;
        private readonly ITransactionRepository _transactionRepo;
        private readonly IUnitOfWork _uow;

        public TransferHandler(IWalletRepository walletRepo, ITransactionRepository transactionRepo, IUnitOfWork uow)
        {
            _walletRepo = walletRepo;
            _transactionRepo = transactionRepo;
            _uow = uow;
        }

        public async Task<OneOf<TransferResponse, AppError>> Handle(TransferCommand command, CancellationToken ct)
        {
            var userWallet = await _walletRepo.FindWalletByUserIdAsync(command.UserId, ct);
            var toWallet = await _walletRepo.FindWalletByAccountNumberAsync(command.AccountNumber, ct);

            if (userWallet is null)
                return new WalletNotFoundError();

            if (toWallet is null)
                return new AccountNotFoundError();
            
            if (toWallet.UserId == userWallet.UserId)
                return new InvalidTransferError();

            if (userWallet.Balance < command.Amount)
                return new InsufficientFundsError();

            userWallet.Debit(command.Amount);
            toWallet.Credit(command.Amount);

            var newTransaction = new Transaction(
                userWallet.Id,
                toWallet.Id,
                command.Amount,
                TransactionType.TransferP2P
            );

            await _transactionRepo.Add(newTransaction);
            await _uow.CommitAsync(ct);

            return new TransferResponse(
                userWallet.AccountNumber,
                toWallet.AccountNumber,
                newTransaction.Amount,
                DateTime.UtcNow
            );
        }
    }
}