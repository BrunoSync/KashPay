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
using Microsoft.Extensions.Logging;
using OneOf;

namespace KashPay.Application.Features.Wallet.Commands.Transfer
{
    public class TransferHandler : IRequestHandler<TransferCommand, OneOf<TransferResponse, AppError>>
    {
        private readonly IWalletRepository _walletRepo;
        private readonly ITransactionRepository _transactionRepo;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<TransferHandler> _logger;

        public TransferHandler(IWalletRepository walletRepo, ITransactionRepository transactionRepo, IUnitOfWork uow, ILogger<TransferHandler> logger)
        {
            _walletRepo = walletRepo;
            _transactionRepo = transactionRepo;
            _uow = uow;
            _logger = logger;
        }

        public async Task<OneOf<TransferResponse, AppError>> Handle(TransferCommand command, CancellationToken ct)
        {
            // Transaction
            await _uow.BeginTransactionAsync(ct);

            var userWalletId = await _walletRepo.GetWalletIdByUserIdAsync(command.UserId, ct);
            var toWalletId = await _walletRepo.GetWalletIdByAccountNumberAsync(command.AccountNumber, ct);
            
            if (userWalletId is null)
            {
                _logger.LogWarning("User wallet not found");
                return new WalletNotFoundError();
            }

            if (toWalletId is null)
            {
                _logger.LogWarning("To wallet not found");
                return new AccountNotFoundError();
            }

            var walletsIds = new[] {userWalletId!.Value, toWalletId!.Value}
                .OrderBy(id => id)
                .ToArray();

            var firstWallet = await _walletRepo.FindWalletLockByIdAsync(walletsIds[0], ct); 
            var secondWallet = await _walletRepo.FindWalletLockByIdAsync(walletsIds[1], ct);
            KashPay.Domain.Entities.Wallet fromWalletAccount;
            KashPay.Domain.Entities.Wallet toWalletAccount;

            if (firstWallet is null)
            {
                _logger.LogWarning("First wallet not found");
                return new WalletNotFoundError();
            }

            if (secondWallet is null)
            {
                _logger.LogWarning("Second wallet not found");
                return new AccountNotFoundError();
            }

            if (firstWallet.UserId == command.UserId)
            {
                fromWalletAccount = firstWallet;
                toWalletAccount = secondWallet;
            }
            else
            {
                fromWalletAccount = secondWallet;
                toWalletAccount = firstWallet;
            }
            
            if (fromWalletAccount.UserId == toWalletAccount.UserId)
            {
                _logger.LogWarning("Cannot transfer to own account");
                return new InvalidTransferError();
            }

            if (fromWalletAccount.Balance < command.Amount)
            {
                _logger.LogWarning("Insufficent funds");
                return new InsufficientFundsError();
            }

            fromWalletAccount.Debit(command.Amount);
            toWalletAccount.Credit(command.Amount);

            var newTransaction = new Domain.Entities.Transaction(
                fromWalletAccount.Id,
                toWalletAccount.Id,
                command.Amount,
                TransactionType.TransferP2P
            );

            await _transactionRepo.Add(newTransaction);
            await _uow.CommitAsync(ct);

            _logger.LogInformation("Successful transfer, fromUser = {fromUserId}, toUser = {toUserId}, amount = {amount}", fromWalletAccount.UserId, toWalletAccount.UserId, command.Amount);

            return new TransferResponse(
                fromWalletAccount.AccountNumber,
                toWalletAccount.AccountNumber,
                newTransaction.Amount,
                DateTime.UtcNow
            );
        }
    }
}