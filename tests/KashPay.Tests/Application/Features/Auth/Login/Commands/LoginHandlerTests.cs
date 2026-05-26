using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using Bogus.Extensions.Brazil;
using FluentAssertions;
using KashPay.Application.Common.Interfaces.Infrastructure;
using KashPay.Application.Common.Interfaces.Infrastructure.Repositories;
using KashPay.Application.Common.Interfaces.Jwt;
using KashPay.Application.Common.Interfaces.Repositories;
using KashPay.Application.Common.OneOf.Errors;
using KashPay.Application.Features.Auth.Login.Queries;
using KashPay.Domain.Entities;
using NSubstitute;
using NSubstitute.Core.Arguments;
using NSubstitute.ReturnsExtensions;
using Xunit.Sdk;

namespace KashPay.Tests.Application.Features.Auth.Login.Commands
{
    public class LoginHandlerTests
    {
        private readonly Faker _faker = new("pt_BR");
        private readonly ICpfHasher _cpfHasher;
        private readonly IUnitOfWork _uow;
        private readonly IPasswordHasher _passHasher;
        private readonly IJwtService _token;
        private readonly IUserRepository _userRepo;
        private readonly IRefreshTokenRepository _rtRepo;
        private readonly LoginHandler _handler;

        public LoginHandlerTests()
        {
            _cpfHasher = Substitute.For<ICpfHasher>();
            _uow = Substitute.For<IUnitOfWork>();
            _passHasher = Substitute.For<IPasswordHasher>();
            _token = Substitute.For<IJwtService>();
            _userRepo = Substitute.For<IUserRepository>();
            _rtRepo = Substitute.For<IRefreshTokenRepository>();
            _handler = new LoginHandler(_cpfHasher, _uow, _passHasher, _token, _userRepo, _rtRepo);
        }

        [Fact]
        [Trait("Features", "Login")]
        public async Task Handle_ShouldReturnLoginResponse_WhenValidEmailIsProvided()
        {
            var command = new LoginCommand(
                _faker.Internet.Email().Trim().ToLower(),
                _faker.Internet.Password(10)
            );

            var user = CreateUser();

            _userRepo.GetUserByCredentialsAsync(command.Credentials, Arg.Any<CancellationToken>())
                .Returns(user);

            _passHasher.Validate(command.Password, user.HashPassword)
                .Returns(true);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsT0.Should().BeTrue();
            var success = result.AsT0;
            success.Should().BeOfType<LoginResponse>();
        }

        [Fact]
        [Trait("Features", "Login")]
        public async Task Handle_ShouldReturnLoginResponse_WhenValidCpfIsProvided()
        {
            var (command, user) = SetupValidCpfLogin();

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsT0.Should().BeTrue();
            var success = result.AsT0;
            success.Should().BeOfType<LoginResponse>();
        }

        [Fact]
        [Trait("Features", "Login")]
        public async Task Handle_ShouldReturnInvalidCredentialsError_WhenUserNotFound()
        {
            var command = new LoginCommand(
                _faker.Person.Cpf(false),
                _faker.Internet.Password(10)
            );

            var normalizedCpf = _cpfHasher.Hash(command.Credentials);

            _userRepo.GetUserByCredentialsAsync(normalizedCpf, Arg.Any<CancellationToken>())
                .ReturnsNull();

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsT1.Should().BeTrue();
            var error = result.AsT1;
            error.Should().BeOfType<InvalidCredentialsError>();
        }

        [Fact]
        [Trait("Features", "Login")]
        public async Task Handle_ShouldReturnInvalidCredentialsError_WhenPasswordIsWrong()
        {
            var command = new LoginCommand(
                _faker.Person.Cpf(false),
                _faker.Internet.Password(10)
            );

            var user = CreateUser();

            var normalizedCpf = _cpfHasher.Hash(command.Credentials);

            _userRepo.GetUserByCredentialsAsync(normalizedCpf, Arg.Any<CancellationToken>())
                .Returns(user);

            _passHasher.Validate(command.Password, user.HashPassword)
                .Returns(false);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsT1.Should().BeTrue();
            var error = result.AsT1;
            error.Should().BeOfType<InvalidCredentialsError>();
        }

        [Fact]
        [Trait("Features", "Login")]
        public async Task Handle_ShouldHashCpf_WhenCredentialsIsCpf()
        {
            var command = new LoginCommand(
                _faker.Person.Cpf(false),
                _faker.Internet.Password(10)
            );

            await _handler.Handle(command, CancellationToken.None);

            _cpfHasher.Received(1).Hash(command.Credentials);
        }

        [Fact]
        [Trait("Features", "Login")]
        public async Task Handle_ShouldNotHashCpf_WhenCredentialsIsEmail()
        {
            var command = new LoginCommand(
                _faker.Internet.Email(),
                _faker.Internet.Password(10)
            );

            await _handler.Handle(command, CancellationToken.None);

            _cpfHasher.Received(0).Hash(command.Credentials);
        }

        [Fact]
        [Trait("Features", "Login")]
        public async Task Handle_ShouldGenerateTokens_WhenCredentialsAreValid()
        {
            var (command, user) = SetupValidCpfLogin();

            await _handler.Handle(command, CancellationToken.None);

            _token.Received(1).GenerateAccessToken(user);
            _token.Received(1).GenerateRefreshToken();
        }

        [Fact]
        [Trait("Features", "Login")]
        public async Task Handle_ShouldCommit_WhenLoginIsSuccessful()
        {
            var (command, user) = SetupValidCpfLogin();

            await _handler.Handle(command, CancellationToken.None);

            await _uow.Received(1).CommitAsync(Arg.Any<CancellationToken>());
        }

        private User CreateUser()
        => new User(
            _faker.Name.FirstName().Replace(" ", ""),
            _faker.Name.LastName().Replace(" ", ""),
            _faker.Internet.Email(),
            Guid.NewGuid().ToString(),
            Guid.NewGuid().ToString()
        );

        private (LoginCommand command, User user) SetupValidCpfLogin()
        {
            var command = new LoginCommand(_faker.Person.Cpf(false), _faker.Internet.Password(10));
            var user = CreateUser();
            var normalizedCpf = _cpfHasher.Hash(command.Credentials);

            _userRepo.GetUserByCredentialsAsync(normalizedCpf, Arg.Any<CancellationToken>()).Returns(user);
            _passHasher.Validate(command.Password, user.HashPassword).Returns(true);

            return (command, user);
        }
    }
}