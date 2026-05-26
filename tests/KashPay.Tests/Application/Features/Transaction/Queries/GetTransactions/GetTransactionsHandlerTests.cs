using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using KashPay.Application.Common.Interfaces.Infrastructure.Repositories;
using KashPay.Application.Common.OneOf.Errors;
using KashPay.Application.Features.Transaction.Queries.GetTransactions;
using KashPay.Domain.Enums;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace KashPay.Tests.Application.Features.Transaction.Queries.GetTransactions
{
    public class GetTransactionsHandlerTests
    {
        private readonly IWalletRepository _walletRepo;
        private readonly ITransactionRepository _transactionRepo;
        private readonly GetTransactionsHandler _handler;

        public GetTransactionsHandlerTests()
        {
            _walletRepo = Substitute.For<IWalletRepository>();
            _transactionRepo = Substitute.For<ITransactionRepository>();
            _handler = new GetTransactionsHandler(_walletRepo, _transactionRepo);
        }

        [Fact]
        [Trait("Features", "GetTransactions")]
        public async Task Handle_ShouldReturnGetTransactionsResponse_WhenWalletExists()
        {
            var (query, wallet, transactions) = SetupWithTransactions(15, 2);

            var result = await _handler.Handle(query, CancellationToken.None);

            result.IsT0.Should().BeTrue();
            var success = result.AsT0;
            success.Should().BeOfType<GetTransactionsResponse>();
            success.Transactions.Should().HaveCount(2);
        }

        [Fact]
        [Trait("Features", "GetTransactions")]
        public async Task Handle_ShouldReturnWalletNotFoundError_WhenWalletDoesNotExist()
        {
            var query = new GetTransactionsQuery(Guid.NewGuid(), 5, null);

            _walletRepo.FindWalletByUserIdAsync(query.UserId, Arg.Any<CancellationToken>())
                .ReturnsNull();

            var result = await _handler.Handle(query, CancellationToken.None);

            result.IsT1.Should().BeTrue();
            var error = result.AsT1;
            error.Should().BeOfType<WalletNotFoundError>();
        }

        [Fact]
        [Trait("Features", "GetTransactions")]
        public async Task Handle_ShouldReturnNextCursor_WhenThereAreMoreTransactions()
        {
            var (query, wallet, transactions) = SetupWithTransactions(2, 2);

            var result = await _handler.Handle(query, CancellationToken.None);

            result.IsT0.Should().BeTrue();
            var success = result.AsT0;
            success.Cursor.Should().NotBeNull();
            success.Cursor.Should().Be(((DateTime?)transactions.Last().CreatedAt, (Guid?)transactions.Last().Id));
        }

        [Fact]
        [Trait("Features", "GetTransactions")]
        public async Task Handle_ShouldReturnNullCursor_WhenThereAreNoMoreTransactions()
        {
            var (query, wallet, transactions) = SetupWithTransactions(15, 10);

            var result = await _handler.Handle(query, CancellationToken.None);

            result.IsT0.Should().BeTrue();
            var success = result.AsT0;
            success.Cursor.Should().BeNull();
        }

        private (GetTransactionsQuery query, KashPay.Domain.Entities.Wallet wallet, List<KashPay.Domain.Entities.Transaction> transactions) SetupWithTransactions(int pageSize, int transactionCount)
        {
            var query = new GetTransactionsQuery(Guid.NewGuid(), pageSize, null);
            var wallet = new KashPay.Domain.Entities.Wallet(query.UserId);
            var transactions = Enumerable.Range(0, transactionCount)
                .Select((_, i) => new KashPay.Domain.Entities.Transaction(wallet.Id, null, (i + 1) * 100, TransactionType.Deposit))
                .ToList();

            _walletRepo.FindWalletByUserIdAsync(query.UserId, Arg.Any<CancellationToken>()).Returns(wallet);
            _transactionRepo.GetByWalletIdAsync(wallet.Id, query.PageSize, query.Cursor, Arg.Any<CancellationToken>()).Returns(transactions);

            return (query, wallet, transactions);
        }
    }
}