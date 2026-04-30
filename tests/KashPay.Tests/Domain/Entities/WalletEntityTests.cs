using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using KashPay.Domain.Entities;

namespace KashPay.Tests.Domain.Entities
{
    public class WalletEntityTests
    {
        [Fact]
        [Trait("Domain", "Entities")]
        public void Constructor_ShouldSetProperties_WhenValidDataIsProvided()
        {
            var expectedUserId = Guid.NewGuid();
            
            var wallet = new Wallet(expectedUserId);

            wallet.Id.Should().NotBeEmpty();
            wallet.UserId.Should().Be(expectedUserId);
            wallet.AccountNumber.Should().NotBeEmpty();
            wallet.Balance.Should().Be(0);
            wallet.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        [Trait("Domain", "Entities")]
        public void Constructor_ShouldGenerateUniqueId_WhenCalledMultipleTimes()
        {
            var wallet1 = new Wallet(Guid.NewGuid());
            var wallet2 = new Wallet(Guid.NewGuid());

            wallet1.Id.Should().NotBe(wallet2.Id);
        }

        [Fact]
        [Trait("Domain", "Entities")]
        public void Credit_ShouldIncreaseBalance_WhenValidAmountIsProvided()
        {
            var wallet = new Wallet(Guid.NewGuid());

            wallet.Credit(100);

            wallet.Balance.Should().Be(100);
        }

        [Fact]
        [Trait("Domain", "Entities")]
        public void Constructor_ShouldGenerateUniqueAccountNumber_WhenCalledMultipleTimes()
        {
            var wallet1 = new Wallet(Guid.NewGuid());
            var wallet2 = new Wallet(Guid.NewGuid());

            wallet1.Id.Should().NotBe(wallet2.Id);
        }

        [Fact]
        [Trait("Domain", "Entities")]
        public void Debit_ShouldDecreaseBalance_WhenValidAmountIsProvided()
        {
            var wallet = new Wallet(Guid.NewGuid());

            wallet.Credit(100);
            wallet.Debit(30);

            wallet.Balance.Should().Be(70);
        }
    }
}