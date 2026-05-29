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

namespace KashPay.Application.Features.Wallet.Commands.Deposit
{
    public class DepositHandler : IRequestHandler<DepositCommand, OneOf<DepositResponse, AppError>>
    {
        private readonly IWalletRepository _walletRepo;
        private readonly ITransactionRepository _transactionRepo;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<DepositHandler> _logger;

        public DepositHandler(IWalletRepository walletRepo, ITransactionRepository transactionRepo, IUnitOfWork uow, ILogger<DepositHandler> logger)
        {
            _walletRepo = walletRepo;
            _transactionRepo = transactionRepo;
            _uow = uow;
            _logger = logger;
        }

        public async Task<OneOf<DepositResponse, AppError>> Handle(DepositCommand command, CancellationToken ct)
        {
            var wallet = await _walletRepo.FindWalletByUserIdAsync(command.userId, ct);

            if (wallet is null)
            {
                _logger.LogWarning("Wallet not found");
                return new WalletNotFoundError();
            }

            wallet.Credit(command.Amount);

            var newTransaction = new Domain.Entities.Transaction(
                null,
                wallet.Id,
                command.Amount,
                TransactionType.Deposit
            );

            await _transactionRepo.Add(newTransaction);
            await _uow.CommitAsync(ct);

            _logger.LogInformation("Deposit completed, userId = {id} - amount = {amount}", wallet.UserId, command.Amount);

            return new DepositResponse(
                newTransaction.Id,
                newTransaction.Amount,
                wallet.Balance,
                DateTime.UtcNow
            );
        }
    }
}