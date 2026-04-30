using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using KashPay.Application.Common.Interfaces.Infrastructure.Repositories;
using KashPay.Application.Common.OneOf.Errors;
using KashPay.Application.Features.Wallet.Commands.Transfer;
using KashPay.Domain.Entities;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace KashPay.Tests.Application.Features.Wallet.Commands.Transfer
{
    public class TransferHandlerTests
    {
        private readonly IWalletRepository _walletRepo;
        private readonly ITransactionRepository _transactionRepo;
        private readonly IUnitOfWork _uow;
        private readonly TransferHandler _handler;

        public TransferHandlerTests()
        {
            _walletRepo = Substitute.For<IWalletRepository>();
            _transactionRepo = Substitute.For<ITransactionRepository>();
            _uow = Substitute.For<IUnitOfWork>();
            _handler = new TransferHandler(_walletRepo, _transactionRepo, _uow);
        }

        [Fact]
        [Trait("Features", "Transfer")]
        public async Task Handle_ShouldReturnTransferResponse_WhenTransferIsSuccessful()
        {
            var command = new TransferCommand(Guid.NewGuid(), AccountNumberGenerator(), Random.Shared.Next(1, 1000));
            var fromWallet = new KashPay.Domain.Entities.Wallet(command.UserId);
            fromWallet.Credit(10000);

            var toWallet = new KashPay.Domain.Entities.Wallet(Guid.NewGuid());

            _walletRepo.FindWalletByUserIdAsync(command.UserId, Arg.Any<CancellationToken>())
                .Returns(fromWallet);
            _walletRepo.FindWalletByAccountNumberAsync(command.AccountNumber, Arg.Any<CancellationToken>()) 
                .Returns(toWallet);
            
            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsT0.Should().BeTrue();
            var success = result.AsT0;
            success.Should().BeOfType<TransferResponse>();

            await _transactionRepo.Received(1).Add(Arg.Any<KashPay.Domain.Entities.Transaction>());
            await _uow.Received(1).CommitAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        [Trait("Features", "Transfer")]
        public async Task Handle_ShouldReturnWalletNotFoundError_WhenUserWalletDoesNotExist()
        {
            var command = new TransferCommand(Guid.NewGuid(), AccountNumberGenerator(), Random.Shared.Next(1, 1000));

            _walletRepo.FindWalletByUserIdAsync(command.UserId, Arg.Any<CancellationToken>())
                .ReturnsNull();
            
            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsT1.Should().BeTrue();
            var error = result.AsT1;
            error.Should().BeOfType<WalletNotFoundError>();
        }

        [Fact]
        [Trait("Features", "Transfer")]
        public async Task Handle_ShouldReturnAccountNotFoundError_WhenDestinationWalletDoesNotExist()
        {
            var command = new TransferCommand(Guid.NewGuid(), AccountNumberGenerator(), Random.Shared.Next(1, 1000));
            var fromWallet = new KashPay.Domain.Entities.Wallet(command.UserId);

            _walletRepo.FindWalletByUserIdAsync(command.UserId, Arg.Any<CancellationToken>())
                .Returns(fromWallet);
            _walletRepo.FindWalletByAccountNumberAsync(command.AccountNumber, Arg.Any<CancellationToken>()) 
                .ReturnsNull();
            
            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsT1.Should().BeTrue();
            var error = result.AsT1;
            error.Should().BeOfType<AccountNotFoundError>();
        }

        [Fact]
        [Trait("Features", "Transfer")]
        public async Task Handle_ShouldReturnInvalidTransferError_WhenTransferringToSameAccount()
        {
            var command = new TransferCommand(Guid.NewGuid(), AccountNumberGenerator(), Random.Shared.Next(1, 1000));
            var fromWallet = new KashPay.Domain.Entities.Wallet(command.UserId);
            fromWallet.Credit(10000);

            var toWallet = new KashPay.Domain.Entities.Wallet(command.UserId);

            _walletRepo.FindWalletByUserIdAsync(command.UserId, Arg.Any<CancellationToken>())
                .Returns(fromWallet);
            _walletRepo.FindWalletByAccountNumberAsync(command.AccountNumber, Arg.Any<CancellationToken>()) 
                .Returns(toWallet);
            
            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsT1.Should().BeTrue();
            var error = result.AsT1;
            error.Should().BeOfType<InvalidTransferError>();
        }

        [Fact]
        [Trait("Features", "Transfer")]
        public async Task Handle_ShouldReturnInsufficientFundsError_WhenBalanceIsLow()
        {
            var command = new TransferCommand(Guid.NewGuid(), AccountNumberGenerator(), Random.Shared.Next(1, 1000));
            var fromWallet = new KashPay.Domain.Entities.Wallet(command.UserId);

            var toWallet = new KashPay.Domain.Entities.Wallet(Guid.NewGuid());

            _walletRepo.FindWalletByUserIdAsync(command.UserId, Arg.Any<CancellationToken>())
                .Returns(fromWallet);
            _walletRepo.FindWalletByAccountNumberAsync(command.AccountNumber, Arg.Any<CancellationToken>()) 
                .Returns(toWallet);
            
            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsT1.Should().BeTrue();
            var error = result.AsT1;
            error.Should().BeOfType<InsufficientFundsError>();
            toWallet.Balance.Should().Be(0);
        }

        [Fact]
        [Trait("Features", "Transfer")]
        public async Task Handle_ShouldDebitAndCreditWallets_WhenTransferIsSuccessful()
        {
            var command = new TransferCommand(Guid.NewGuid(), AccountNumberGenerator(), Random.Shared.Next(1, 1000));
            var fromWallet = new KashPay.Domain.Entities.Wallet(command.UserId);
            fromWallet.Credit(10000);

            var toWallet = new KashPay.Domain.Entities.Wallet(Guid.NewGuid());

            _walletRepo.FindWalletByUserIdAsync(command.UserId, Arg.Any<CancellationToken>())
                .Returns(fromWallet);
            _walletRepo.FindWalletByAccountNumberAsync(command.AccountNumber, Arg.Any<CancellationToken>()) 
                .Returns(toWallet);
            
            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsT0.Should().BeTrue();
            var success = result.AsT0;
            success.Should().BeOfType<TransferResponse>();
            toWallet.Balance.Should().Be(command.Amount);
        }

        private string AccountNumberGenerator()
        {
            var number = Random.Shared.Next(10000, 9999999);
            var digit = Random.Shared.Next(10, 99);

            return $"{number}-{digit}";
        }
    }
}