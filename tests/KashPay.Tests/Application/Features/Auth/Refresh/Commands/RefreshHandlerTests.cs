using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using KashPay.Application.Common.Interfaces.Infrastructure.Repositories;
using KashPay.Application.Common.Interfaces.Jwt;
using KashPay.Application.Common.Interfaces.Repositories;
using KashPay.Application.Common.OneOf.Errors;
using KashPay.Application.Features.Auth.Refresh.Commands;
using KashPay.Domain.Entities;
using Microsoft.VisualBasic;
using NSubstitute;
using NSubstitute.ReturnsExtensions;

namespace KashPay.Tests.Application.Features.Auth.Refresh.Commands
{
    public class RefreshHandlerTests
    {
        private readonly Faker _faker = new("pt_BR");
        private readonly IRefreshTokenRepository _rtRepo;
        private readonly IUserRepository _userRepo;
        private readonly IJwtService _token;
        private readonly IUnitOfWork _uow;
        private readonly RefreshHandler _handler;

        public RefreshHandlerTests()
        {
            _rtRepo = Substitute.For<IRefreshTokenRepository>();
            _userRepo = Substitute.For<IUserRepository>();
            _token = Substitute.For<IJwtService>();
            _uow = Substitute.For<IUnitOfWork>();
            _handler = new RefreshHandler(_rtRepo, _userRepo, _token, _uow);
        }

        [Fact]
        [Trait("Features", "Refresh")]
        public async Task Handle_ShouldReturnRefreshResponse_WhenValidTokenIsProvided()
        {
            var (command, token, user) = SetupValidRefresh();

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsT0.Should().BeTrue();
            var success = result.AsT0;
            success.Should().BeOfType<RefreshResponse>();
        }

        [Fact]
        [Trait("Features", "Refresh")]
        public async Task Handle_ShouldReturnRefreshTokenNotFoundError_WhenTokenDoesNotExist()
        {
            var command = new RefreshCommand(new string('a', 32));

            _rtRepo.GetRefreshTokenByTokenAsync(command.RefreshToken, Arg.Any<CancellationToken>())
                .ReturnsNull();
                
            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsT1.Should().BeTrue();
            var error = result.AsT1;
            error.Should().BeOfType<RefreshTokenNotFoundError>();
        }

        [Fact]
        [Trait("Features", "Refresh")]
        public async Task Handle_ShouldReturnInvalidRefreshTokenError_WhenTokenIsRevoked()
        {
            var command = new RefreshCommand(new string('a', 32));

            var expectedRt = new RefreshToken(
                Guid.NewGuid(),
                new string('a', 32),
                DateTime.UtcNow.AddDays(7)
            );

            expectedRt.Revoke();

            _rtRepo.GetRefreshTokenByTokenAsync(command.RefreshToken, Arg.Any<CancellationToken>())
                .Returns(expectedRt);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsT1.Should().BeTrue();
            var error = result.AsT1;
            error.Should().BeOfType<InvalidRefreshTokenError>();
        }

        [Fact]
        [Trait("Features", "Refresh")]
        public async Task Handle_ShouldReturnInvalidRefreshTokenError_WhenTokenIsExpired()
        {
            var command = new RefreshCommand(new string('a', 32));

            var expectedRt = new RefreshToken(
                Guid.NewGuid(),
                new string('a', 32),
                DateTime.UtcNow.AddDays(-2)
            );

            _rtRepo.GetRefreshTokenByTokenAsync(command.RefreshToken, Arg.Any<CancellationToken>())
                .Returns(expectedRt);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsT1.Should().BeTrue();
            var error = result.AsT1;
            error.Should().BeOfType<InvalidRefreshTokenError>();
        }

        [Fact]
        [Trait("Features", "Refresh")]
        public async Task Handle_ShouldReturnUserNotFoundError_WhenUserDoesNotExist()
        {
            var command = new RefreshCommand(new string('a', 32));

            var token = CreateValidToken();

            _rtRepo.GetRefreshTokenByTokenAsync(command.RefreshToken, Arg.Any<CancellationToken>())
                .Returns(token);

            _userRepo.GetUserByIdAsync(token.UserId, Arg.Any<CancellationToken>())
                .ReturnsNull();

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsT1.Should().BeTrue();
            var error = result.AsT1;
            error.Should().BeOfType<UserNotFoundError>();
        }

        [Fact]
        [Trait("Features", "Refresh")]
        public async Task Handle_ShouldRevokeOldToken_WhenTokenIsValid()
        {
            var (command, token, user) = SetupValidRefresh();

            await _handler.Handle(command, CancellationToken.None);

            token.IsRevoked.Should().BeTrue();
        }

        [Fact]
        [Trait("Features", "Refresh")]
        public async Task Handle_ShouldGenerateNewTokens_WhenTokenIsValid()
        {
            var (command, token, user) = SetupValidRefresh();

            await _handler.Handle(command, CancellationToken.None);

            _token.Received(1).GenerateAccessToken(user);
            _token.Received(1).GenerateRefreshToken();
        }

        [Fact]
        [Trait("Features", "Refresh")]
        public async Task Handle_ShouldCommit_WhenRefreshIsSuccessful()
        {
            var (command, token, user) = SetupValidRefresh();

            await _handler.Handle(command, CancellationToken.None);

            await _uow.Received(1).CommitAsync(Arg.Any<CancellationToken>());
        }

        private RefreshToken CreateValidToken() =>
            new(Guid.NewGuid(), new string('a', 32), DateTime.UtcNow.AddDays(7));

        private User CreateUser() =>
            new(_faker.Name.FirstName(), _faker.Name.LastName(),
                _faker.Internet.Email(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
    
        private (RefreshCommand command, RefreshToken token, User user) SetupValidRefresh()
        {
            var command = new RefreshCommand(new string('a', 32));
            var token = CreateValidToken();
            var user = CreateUser();

            _rtRepo.GetRefreshTokenByTokenAsync(command.RefreshToken, Arg.Any<CancellationToken>()).Returns(token);
            _userRepo.GetUserByIdAsync(token.UserId, Arg.Any<CancellationToken>()).Returns(user);

            return (command, token, user);
        }

    }
}