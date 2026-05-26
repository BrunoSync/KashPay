using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using KashPay.Application.Common.Interfaces.Infrastructure.Repositories;
using KashPay.Application.Features.Auth.Logout.Commands;
using KashPay.Domain.Entities;
using NSubstitute;

namespace KashPay.Tests.Application.Features.Auth.Logout
{
    public class LogoutHandlerTests
    {
        private readonly IRefreshTokenRepository _rtRepo;
        private readonly IUnitOfWork _uow;
        private readonly LogoutHandler _handler;

        public LogoutHandlerTests()
        {
            _rtRepo = Substitute.For<IRefreshTokenRepository>();
            _uow = Substitute.For<IUnitOfWork>();
            _handler = new LogoutHandler(_rtRepo, _uow);
        }

        [Fact]
        [Trait("Features", "Logout")]
        public async Task Handle_ShouldRevokeAllValidTokens_WhenUserHasActiveTokens()
        {
            var command = new LogoutCommand(Guid.NewGuid());

            List<RefreshToken> tokens = CreateTokens();

            _rtRepo.GetAllValidTokensByUserAsync(command.UserId, Arg.Any<CancellationToken>())
                .Returns(tokens);

            await _handler.Handle(command, CancellationToken.None);

            tokens.Should().AllSatisfy(e => e.IsRevoked.Should().BeTrue());
        }

        [Fact]
        [Trait("Features", "Logout")]
        public async Task Handle_ShouldCommit_WhenLogoutIsSuccessful()
        {
            var command = new LogoutCommand(Guid.NewGuid());

            List<RefreshToken> tokens = CreateTokens();

            _rtRepo.GetAllValidTokensByUserAsync(command.UserId, Arg.Any<CancellationToken>())
                .Returns(tokens);

            await _handler.Handle(command, CancellationToken.None);

            await _uow.Received(1).CommitAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        [Trait("Features", "Logout")]
        public async Task Handle_ShouldCommit_EvenWhenUserHasNoActiveTokens()
        {
            var command = new LogoutCommand(Guid.NewGuid());

            List<RefreshToken> tokens = new();

            _rtRepo.GetAllValidTokensByUserAsync(command.UserId, Arg.Any<CancellationToken>())
                .Returns(tokens);

            await _handler.Handle(command, CancellationToken.None);

            await _uow.Received(1).CommitAsync(Arg.Any<CancellationToken>());
        }

        private List<RefreshToken> CreateTokens(int count = 4)
        => Enumerable.Range(0, count)
                .Select(_ => new RefreshToken(Guid.NewGuid(), Guid.NewGuid().ToString(), DateTime.UtcNow.AddDays(7)))
                .ToList();
    }
}