using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using KashPay.Application.Common.Interfaces.Infrastructure.Repositories;
using KashPay.Application.Common.OneOf.Errors;
using KashPay.Application.Features.Wallet.Queries.GetBalance;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace KashPay.Tests.Application.Features.Wallet.Queries.GetBalance
{
    public class GetbalanceHandlerTests
    {
        private readonly IWalletRepository _walletRepo;
        private readonly GetBalanceHandler _handler;

        public GetbalanceHandlerTests()
        {
            _walletRepo = Substitute.For<IWalletRepository>();
            _handler = new GetBalanceHandler(_walletRepo);
        }

        [Fact]
        [Trait("Features", "GetBalance")]
        public async Task Handle_ShouldReturnGetBalanceResponse_WhenWalletExists()
        {
            var query = new GetBalanceQuery(Guid.NewGuid());
            var wallet = new KashPay.Domain.Entities.Wallet(query.UserId);
            var balance = Random.Shared.Next(1, 10000);

            wallet.Credit(balance);

            _walletRepo.GetWalletByUserIdAsync(wallet.UserId, Arg.Any<CancellationToken>())
                .Returns(wallet);

            var result = await _handler.Handle(query, CancellationToken.None);

            result.IsT0.Should().BeTrue();
            var success = result.AsT0;
            success.Balance.Should().Be(balance);
        }

        [Fact]
        [Trait("Features", "GetBalance")]
        public async Task Handle_ShouldReturnWalletNotFoundError_WhenWalletDoesNotExist()
        {
            var query = new GetBalanceQuery(Guid.NewGuid());

            _walletRepo.GetWalletByUserIdAsync(query.UserId, Arg.Any<CancellationToken>())
                .ReturnsNull();

            var result = await _handler.Handle(query, CancellationToken.None);

            result.IsT1.Should().BeTrue();
            var error = result.AsT1;
            error.Should().BeOfType<WalletNotFoundError>();
        }
    }
}