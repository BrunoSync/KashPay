using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using KashPay.Domain.Entities;
using KashPay.Domain.Enums;

namespace KashPay.Tests.Domain.Entities
{
    public class TransactionEntityTests
    {
        [Fact]
        [Trait("Domain", "Entities")]
        public void Constructor_ShouldSetProperties_WhenValidDataIsProvided()
        {
            var transaction = new Transaction(null, null, 100, TransactionType.Deposit);

            transaction.Id.Should().NotBeEmpty();
            transaction.FromAccountId.Should().BeNull();
            transaction.ToAccountId.Should().BeNull();
            transaction.Amount.Should().Be(100);
            transaction.Type.Should().Be(TransactionType.Deposit);
            transaction.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        [Trait("Domain", "Entities")]
        public void Constructor_ShouldGenerateUniqueId_WhenCalledMultipleTimes()
        {
            var transaction1 = new Transaction(null, null, 100, TransactionType.Deposit);
            var transaction2 = new Transaction(null, null, 100, TransactionType.Deposit);
            
            transaction1.Id.Should().NotBe(transaction2.Id);
        }
    }
}