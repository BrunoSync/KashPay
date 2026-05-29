using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KashPay.Application.Common.Interfaces.Infrastructure.Repositories;
using KashPay.Application.Common.OneOf;
using KashPay.Application.Common.OneOf.Errors;
using KashPay.Application.Features.Wallet.Commands.Deposit;
using KashPay.Domain.Entities;
using KashPay.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using OneOf;

namespace KashPay.Application.Features.Wallet.Commands.Withdraw
{
    public class WithdrawHandler : IRequestHandler<WithdrawCommand, OneOf<WithdrawResponse, AppError>>
    {
        private readonly IWalletRepository _walletRepo;
        private readonly ITransactionRepository _transactionRepo;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<WithdrawHandler> _logger;

        public WithdrawHandler(IWalletRepository walletRepo, ITransactionRepository transactionRepo, IUnitOfWork uow, ILogger<WithdrawHandler> logger)
        {
            _walletRepo = walletRepo;
            _transactionRepo = transactionRepo;
            _uow = uow;
            _logger = logger;
        }

        public async Task<OneOf<WithdrawResponse, AppError>> Handle(WithdrawCommand command, CancellationToken ct)
        {
            var wallet = await _walletRepo.FindWalletByUserIdAsync(command.UserId, ct);

            if (wallet is null)
            {
                _logger.LogWarning("Wallet not found");
                return new WalletNotFoundError();
            }

            if (wallet.Balance < command.Amount)
            {
                _logger.LogWarning("Insufficient funds");
                return new InsufficientFundsError();
            }

            wallet.Debit(command.Amount);

            var newTransaction = new Domain.Entities.Transaction(
                wallet.Id,
                null,
                command.Amount,
                TransactionType.WithDraw
            );

            await _transactionRepo.Add(newTransaction);
            await _uow.CommitAsync(ct);

            _logger.LogInformation("Successful withdraw, userId = {userId}, amount = {amount}", wallet.UserId, command.Amount);

            return new WithdrawResponse(
                newTransaction.Id,
                newTransaction.Amount,
                wallet.Balance,
                DateTime.UtcNow
            );
        }
    }
}