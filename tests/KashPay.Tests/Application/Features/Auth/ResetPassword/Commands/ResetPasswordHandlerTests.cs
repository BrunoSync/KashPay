using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using KashPay.Application.Common.Interfaces.Infrastructure;
using KashPay.Application.Common.Interfaces.Infrastructure.Repositories;
using KashPay.Application.Common.OneOf.Errors;
using KashPay.Application.Features.Auth.ForgetPassword.Commands;
using KashPay.Application.Features.Auth.ResetPassword.Commands;
using KashPay.Domain.Entities;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace KashPay.Tests.Application.Features.Auth.ResetPassword.Commands
{
    public class ResetPasswordHandlerTests
    {
        private readonly IPasswordResetTokenRepository _tokenRepo;
        private readonly IPasswordHasher _hasher;
        private readonly IUnitOfWork _uow;
        private readonly ResetPasswordHandler _handler;
        private readonly Faker _faker = new("pt_BR");

        public ResetPasswordHandlerTests()
        {
            _tokenRepo = Substitute.For<IPasswordResetTokenRepository>();
            _hasher = Substitute.For<IPasswordHasher>();
            _uow = Substitute.For<IUnitOfWork>();
            _handler = new ResetPasswordHandler(_tokenRepo, _hasher, _uow);
        }

        [Fact]
        [Trait("Features", "ResetPassword")]
        public async Task Handle_ShouldReturnPasswordResetTokenNotFoundError_WhenTokenDoesNotExist()
        {
            var command = CreateCommand();

            _tokenRepo.GetPasswordResetTokenAsync(Arg.Any<string>(), command.Email, Arg.Any<CancellationToken>())
                .ReturnsNull();

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsT1.Should().BeTrue();
            result.AsT1.Should().BeOfType<PasswordResetTokenNotFoundError>();
        }

        [Fact]
        [Trait("Features", "ResetPassword")]
        public async Task Handle_ShouldReturnInvalidPasswordResetTokenError_WhenTokenIsExpired()
        {
            var command = CreateCommand();
            var token = CreateToken(expiresAt: DateTime.UtcNow.AddMinutes(-1));

            _tokenRepo.GetPasswordResetTokenAsync(Arg.Any<string>(), command.Email, Arg.Any<CancellationToken>())
                .Returns(token);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsT1.Should().BeTrue();
            result.AsT1.Should().BeOfType<InvalidPasswordResetTokenError>();
        }

        [Fact]
        [Trait("Features", "ResetPassword")]
        public async Task Handle_ShouldReturnInvalidPasswordResetTokenError_WhenTokenIsRevoked()
        {
            var command = CreateCommand();
            var token = CreateToken();
            token.Revoke();

            _tokenRepo.GetPasswordResetTokenAsync(Arg.Any<string>(), command.Email, Arg.Any<CancellationToken>())
                .Returns(token);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsT1.Should().BeTrue();
            result.AsT1.Should().BeOfType<InvalidPasswordResetTokenError>();
        }

        [Fact]
        [Trait("Features", "ResetPassword")]
        public async Task Handle_ShouldChangePasswordAndRevokeToken_WhenTokenIsValid()
        {
            var (command, token) = SetupValidReset();

            await _handler.Handle(command, CancellationToken.None);

            token.IsRevoked.Should().BeTrue();
            _hasher.Received(1).Hash(command.NewPassword);
        }

        [Fact]
        [Trait("Features", "ResetPassword")]
        public async Task Handle_ShouldCommit_WhenResetIsSuccessful()
        {
            var (command, token) = SetupValidReset();

            await _handler.Handle(command, CancellationToken.None);

            await _uow.Received(1).CommitAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        [Trait("Features", "ResetPassword")]
        public async Task Handle_ShouldReturnResetPasswordResponse_WhenResetIsSuccessful()
        {
            var (command, token) = SetupValidReset();

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsT0.Should().BeTrue();
            result.AsT0.Should().BeOfType<ResetPasswordResponse>();
        }

        private ResetPasswordCommand CreateCommand() => new(
            _faker.Internet.Email(),
            "123456",
            _faker.Internet.Password(10),
            _faker.Internet.Password(10)
        );

        private PasswordResetToken CreateToken(DateTime? expiresAt = null)
        {
            var user = new User(
                _faker.Name.FirstName(),
                _faker.Name.LastName(),
                _faker.Internet.Email(),
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString()
            );
            return new PasswordResetToken(user.Id, "123456", expiresAt ?? DateTime.UtcNow.AddMinutes(30)) { User = user };
        }

        private (ResetPasswordCommand command, PasswordResetToken token) SetupValidReset()
        {
            var command = CreateCommand();
            var token = CreateToken();

            _tokenRepo.GetPasswordResetTokenAsync(Arg.Any<string>(), command.Email, Arg.Any<CancellationToken>())
                .Returns(token);

            return (command, token);
        }
    }
}