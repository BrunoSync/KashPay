using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using KashPay.Domain.Entities;
using Xunit;

namespace KashPay.Tests.Domain.Entities
{
    public class UserEntityTests
    {
        private readonly Faker _faker = new("pt_BR");

        [Fact]
        [Trait("Domain", "Entities")]
        public void Constructor_ShouldSetProperties_WhenValidDataIsProvided()
        {
            // Property preparation
            var expectedFirstName = _faker.Name.FirstName();
            var expectedLastName = _faker.Name.LastName();
            var expectedEmail = _faker.Internet.Email();
            var expectedHashCpf = Guid.NewGuid().ToString();
            var expectedHashPassword = Guid.NewGuid().ToString();

            // User creation
            var user = new User(
                expectedFirstName,
                expectedLastName,
                expectedEmail,
                expectedHashCpf,
                expectedHashPassword
            );

            // Expected properties
            user.Id.Should().NotBeEmpty();
            user.FirstName.Should().Be(expectedFirstName);
            user.LastName.Should().Be(expectedLastName);
            user.Email.Should().Be(expectedEmail);
            user.HashCpf.Should().Be(expectedHashCpf);
            user.HashPassword.Should().Be(expectedHashPassword);
            user.IsBlocked.Should().BeFalse();
            user.BlockedAt.Should().BeNull();
            user.JoinedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }
        
        [Fact]
        [Trait("Domain", "Entities")]
        public void Constructor_ShouldGenerateUniqueId_WhenCalledMultipleTimes()
        {
            // Users creation
            var user1 = new User(
                _faker.Name.FirstName(),
                _faker.Name.LastName(),
                _faker.Internet.Email(),
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString()
            );

            // User creation
            var user2 = new User(
                _faker.Name.FirstName(),
                _faker.Name.LastName(),
                _faker.Internet.Email(),
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString()
            );

            // Expected result
            user1.Id.Should().NotBe(user2.Id);
        }

        [Fact]
        [Trait("Domain", "Entities")]
        public void ChangeHashPassword_ShouldUpdateHashPassword_WhenCalled()
        {
            // User creation
            var user = new User(
                _faker.Name.FirstName(),
                _faker.Name.LastName(),
                _faker.Internet.Email(),
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString()
            );

            // Change Password
            var expectedNewPass = Guid.NewGuid().ToString();
            user.ChangeHashPassword(expectedNewPass);

            // Expected result
            user.HashPassword.Should().Be(expectedNewPass);
        }

        [Fact]
        [Trait("Domain", "Entities")]
        public void SetBlocked_ShouldSetIsBlockedTrue_WhenCalled()
        {
            // User creation
            var user = new User(
                _faker.Name.FirstName(),
                _faker.Name.LastName(),
                _faker.Internet.Email(),
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString()
            );

            // Set blocked
            user.SetBlocked();

            // Expected result
            user.IsBlocked.Should().BeTrue();
            user.BlockedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        [Trait("Domain", "Entities")]
        public void SetUnBlocked_ShouldSetIsBlockedFalse_WhenCalled()
        {
            // User creation
            var user = new User(
                _faker.Name.FirstName(),
                _faker.Name.LastName(),
                _faker.Internet.Email(),
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString()
            );

            // Block and Unblock
            user.SetBlocked();
            user.SetUnBlocked();

            // Expected results
            user.IsBlocked.Should().BeFalse();
            user.BlockedAt.Should().BeNull();
        }
    }
}