using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using KashPay.Application.Common.Interfaces.Infrastructure.Repositories;
using KashPay.Application.Common.OneOf.Errors;
using KashPay.Application.Features.Wallet.Commands.Withdraw;
using KashPay.Domain.Entities;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace KashPay.Tests.Application.Features.Wallet.Commands.Withdraw
{
    public class WithdrawHandlerTests
    {
        private readonly IWalletRepository _walletRepo;
        private readonly ITransactionRepository _transactionRepo;
        private readonly IUnitOfWork _uow;
        private readonly WithdrawHandler _handler;
        
        public WithdrawHandlerTests()
        {
            _walletRepo = Substitute.For<IWalletRepository>();
            _transactionRepo = Substitute.For<ITransactionRepository>();
            _uow = Substitute.For<IUnitOfWork>();
            _handler = new WithdrawHandler(_walletRepo, _transactionRepo, _uow);
        }

        [Fact]
        [Trait("Features", "Withdraw")]
        public async Task Handle_ShouldReturnWithdrawResponse_WhenWalletExistsAndHasFunds()
        {
            var amount = Random.Shared.Next(1, 1000);
            var command = new WithdrawCommand(Guid.NewGuid(), amount);
            var wallet = new KashPay.Domain.Entities.Wallet(command.UserId);

            wallet.Credit(10000);

            _walletRepo.FindWalletByUserIdAsync(command.UserId, Arg.Any<CancellationToken>())
                .Returns(wallet);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsT0.Should().BeTrue();
            var success = result.AsT0;
            success.Should().BeOfType<WithdrawResponse>();

            await _transactionRepo.Received(1).Add(Arg.Any<KashPay.Domain.Entities.Transaction>());
            await _uow.Received(1).CommitAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        [Trait("Features", "Withdraw")]
        public async Task Handle_ShouldReturnWalletNotFoundError_WhenWalletDoesNotExist()
        {
            var amount = Random.Shared.Next(1, 1000);
            var command = new WithdrawCommand(Guid.NewGuid(), amount);

            _walletRepo.FindWalletByUserIdAsync(command.UserId, Arg.Any<CancellationToken>())
                .ReturnsNull();

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsT1.Should().BeTrue();
            var error = result.AsT1;
            error.Should().BeOfType<WalletNotFoundError>();
        }

        [Fact]
        [Trait("Features", "Withdraw")]
        public async Task Handle_ShouldReturnInsufficientFundsError_WhenBalanceIsLow()
        {
            var amount = Random.Shared.Next(1, 1000);
            var command = new WithdrawCommand(Guid.NewGuid(), amount);
            var wallet = new KashPay.Domain.Entities.Wallet(command.UserId);

            _walletRepo.FindWalletByUserIdAsync(command.UserId, Arg.Any<CancellationToken>())
                .Returns(wallet);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsT1.Should().BeTrue();
            var error = result.AsT1;
            error.Should().BeOfType<InsufficientFundsError>();
        }

        [Fact]
        [Trait("Features", "Withdraw")]
        public async Task Handle_ShouldDebitWallet_WhenWithdrawIsSuccessful()
        {
            var command = new WithdrawCommand(Guid.NewGuid(), 1000);
            var wallet = new KashPay.Domain.Entities.Wallet(command.UserId);

            wallet.Credit(10000);

            _walletRepo.FindWalletByUserIdAsync(command.UserId, Arg.Any<CancellationToken>())
                .Returns(wallet);

            await _handler.Handle(command, CancellationToken.None);

            wallet.Balance.Should().Be(9000);
        }
    }
}