using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using KashPay.Application.Common.Interfaces.Infrastructure;
using KashPay.Application.Common.Interfaces.Infrastructure.Repositories;
using KashPay.Application.Common.Interfaces.Repositories;
using KashPay.Application.Features.Auth.ForgotPassword.Commands;
using KashPay.Domain.Entities;
using NSubstitute;
using NSubstitute.Core.Arguments;
using NSubstitute.ReturnsExtensions;

namespace KashPay.Tests.Application.Features.Auth.ForgotPassword.Commands
{
    public class ForgotPasswordHandlerTests
    {
        private readonly IUserRepository _userRepo;
        private readonly IPasswordResetTokenRepository _tokenRepo;
        private readonly IEmailService _email;
        private readonly IUnitOfWork _uow;
        private readonly ForgotPasswordHandler _handler;
        private readonly Faker _faker = new("pt_BR");

        public ForgotPasswordHandlerTests()
        {
            _userRepo = Substitute.For<IUserRepository>();
            _tokenRepo = Substitute.For<IPasswordResetTokenRepository>();
            _email = Substitute.For<IEmailService>();
            _uow = Substitute.For<IUnitOfWork>();
            _handler = new ForgotPasswordHandler(_userRepo, _tokenRepo, _email, _uow);
        }

        [Fact]
        [Trait("Features", "ForgotPassword")]
        public async Task Handle_ShouldReturnGenericResponse_WhenUserExists()
        {
            var command = CreateCommand();
            var user = CreateUser();

            _userRepo.GetUserByCredentialsAsync(command.Email, Arg.Any<CancellationToken>())
                .Returns(user);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().BeOfType<ForgotPasswordResponse>();
        }

        [Fact]
        [Trait("Features", "ForgotPassword")]
        public async Task Handle_ShouldSaveTokenAndSendEmail_WhenUserExists()
        {
            var command = CreateCommand();
            var user = CreateUser();

            _userRepo.GetUserByCredentialsAsync(command.Email, Arg.Any<CancellationToken>())
                .Returns(user);

            var result = await _handler.Handle(command, CancellationToken.None);

            await _tokenRepo.Received(1).AddAsync(Arg.Any<PasswordResetToken>(), Arg.Any<CancellationToken>());
            await _uow.Received(1).CommitAsync(Arg.Any<CancellationToken>());
            await _email.Received(1).SendPasswordResetEmailAsync(command.Email, Arg.Any<string>(), Arg.Any<CancellationToken>());
        }

        [Fact]
        [Trait("Features", "ForgotPassword")]
        public async Task Handle_ShouldReturnGenericResponse_WhenUserDoesNotExist()
        {
            var command = CreateCommand();

            _userRepo.GetUserByCredentialsAsync(command.Email, Arg.Any<CancellationToken>())
                .ReturnsNull();

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().BeOfType<ForgotPasswordResponse>();
            await _tokenRepo.DidNotReceive().AddAsync(Arg.Any<PasswordResetToken>(), Arg.Any<CancellationToken>());
            await _uow.DidNotReceive().CommitAsync(Arg.Any<CancellationToken>());
            await _email.DidNotReceive().SendPasswordResetEmailAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
        }

        private ForgotPasswordCommand CreateCommand()
        => new ForgotPasswordCommand(_faker.Internet.Email());

        private User CreateUser()
        => new User(
            _faker.Person.FirstName,
            _faker.Person.LastName,
            _faker.Internet.Email(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString()
        );
    }
}