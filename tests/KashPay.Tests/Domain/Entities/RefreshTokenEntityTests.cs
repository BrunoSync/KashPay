using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using KashPay.Domain.Entities;

namespace KashPay.Tests.Domain.Entities
{
    public class RefreshTokenEntityTests
    {
        private readonly Faker _faker = new("pt_BR");

        [Fact]
        [Trait("Domain", "Entities")]
        public void Constructor_ShouldSetProperties_WhenValidDataIsProvided()
        {
            var expectedUserId = Guid.NewGuid();
            var expectedToken = _faker.Lorem.Paragraph();
            var expectedExpire = DateTime.UtcNow.AddDays(7);

            var rt = new RefreshToken(expectedUserId, expectedToken, expectedExpire);

            rt.Id.Should().NotBeEmpty();
            rt.UserId.Should().Be(expectedUserId);
            rt.Token.Should().Be(expectedToken);
            rt.ExpiresAt.Should().Be(expectedExpire);
            rt.IsRevoked.Should().BeFalse();
        }

        [Fact]
        [Trait("Domain", "Entities")]
        public void Revoke_ShouldSetIsRevokedTrue_WhenCalled()
        {
            var rt = new RefreshToken(
                Guid.NewGuid(),
                _faker.Lorem.Paragraph(),
                DateTime.UtcNow.AddDays(7)
            );

            rt.Revoke();

            rt.IsRevoked.Should().BeTrue();
        }
    }
}