using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using KashPay.Application.Common.Interfaces.Infrastructure.Repositories;
using KashPay.Application.Common.OneOf.Errors;
using KashPay.Application.Features.Wallet.Commands.Deposit;
using KashPay.Domain.Entities;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace KashPay.Tests.Application.Features.Wallet.Commands.Deposit
{
    public class DepositHandlerTests
    {
        private readonly IWalletRepository _walletRepo;
        private readonly ITransactionRepository _transactionRepo;
        private readonly IUnitOfWork _uow;
        private readonly DepositHandler _handler;

        public DepositHandlerTests()
        {
            _walletRepo = Substitute.For<IWalletRepository>();
            _transactionRepo = Substitute.For<ITransactionRepository>();
            _uow = Substitute.For<IUnitOfWork>();
            _handler = new DepositHandler(_walletRepo, _transactionRepo, _uow);
        }

        [Fact]
        [Trait("Features", "Deposit")]
        public async Task Handle_ShouldReturnDepositResponse_WhenWalletExists()
        {
            var amount = Random.Shared.Next(1, 10000);
            var (command, wallet) = SetupValidDeposit();

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsT0.Should().BeTrue();
            var success = result.AsT0;
            success.Should().BeOfType<DepositResponse>();

            await _transactionRepo.Received(1).Add(Arg.Any<KashPay.Domain.Entities.Transaction>());
            await _uow.Received(1).CommitAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        [Trait("Features", "Deposit")]
        public async Task Handle_ShouldReturnWalletNotFoundError_WhenWalletDoesNotExist()
        {
            var amount = Random.Shared.Next(1, 10000);
            var command = new DepositCommand(Guid.NewGuid(), amount);

            _walletRepo.FindWalletByUserIdAsync(command.userId, Arg.Any<CancellationToken>())
                .ReturnsNull();

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsT1.Should().BeTrue();
            var error = result.AsT1;
            error.Should().BeOfType<WalletNotFoundError>();
        }
        
        [Fact]
        [Trait("Features", "Deposit")]
        public async Task Handle_ShouldCreditWallet_WhenDepositIsSuccessful()
        {
            var (command, wallet) = SetupValidDeposit();

            await _handler.Handle(command, CancellationToken.None);

            wallet.Balance.Should().Be(100);
        }

        private (DepositCommand command, KashPay.Domain.Entities.Wallet wallet) SetupValidDeposit(decimal amount = 100)
        {
            var wallet = new KashPay.Domain.Entities.Wallet(Guid.NewGuid());
            var command = new DepositCommand(wallet.UserId, amount);
            _walletRepo.FindWalletByUserIdAsync(command.userId, Arg.Any<CancellationToken>()).Returns(wallet);
            return (command, wallet);
        }
    }
}