using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using Bogus.Extensions.Brazil;
using FluentAssertions;
using KashPay.Application.Common.Interfaces.Infrastructure;
using KashPay.Application.Common.Interfaces.Infrastructure.Repositories;
using KashPay.Application.Common.Interfaces.Repositories;
using KashPay.Application.Common.OneOf.Errors;
using KashPay.Application.Features.Auth.Login.Register.Commands;
using KashPay.Domain.Entities;
using NSubstitute;

namespace KashPay.Tests.Application.Features.Auth.Register.Commands
{
    public class RegisterHandlerTests
    {
        private readonly IUserRepository _userRepo;
        private readonly IWalletRepository _walletRepo;
        private readonly IUnitOfWork _uow;
        private readonly ICpfHasher _cpfHasher;
        private readonly IPasswordHasher _passwordHasher;
        private readonly RegisterHandler _handler;
        private readonly Faker _faker = new("pt_BR");

        public RegisterHandlerTests()
        {
            _userRepo = Substitute.For<IUserRepository>();
            _walletRepo = Substitute.For<IWalletRepository>();
            _uow = Substitute.For<IUnitOfWork>();
            _cpfHasher = Substitute.For<ICpfHasher>();
            _passwordHasher = Substitute.For<IPasswordHasher>();

            _handler = new RegisterHandler(_userRepo, _walletRepo, _uow, _cpfHasher, _passwordHasher);
        }

        [Fact]
        [Trait("Features", "Register")]
        public async Task Handle_ShouldReturnRegisterResponse_WhenValidDataIsProvided()
        {
            var command = CreateValidSetup();

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsT0.Should().BeTrue();
            var success = result.AsT0;
            success.Id.Should().NotBeEmpty();
            success.Should().BeOfType<RegisterResponse>();
        }

        [Fact]
        [Trait("Features", "Register")]
        public async Task Handle_ShouldReturnEmailAlreadyExistError_WhenEmailIsAlreadyRegistered()
        {
            var command = CreateCommand();

            _userRepo.UserExistByEmailAsync(command.Email, Arg.Any<CancellationToken>())
                .Returns(true);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsT1.Should().BeTrue();
            var error = result.AsT1;
            error.Should().BeOfType<EmailAlreadyExistError>();
        }

        [Fact]
        [Trait("Features", "Register")]
        public async Task Handle_ShouldReturnCpfAlreadyExistError_WhenCpfIsAlreadyRegistered()
        {
            var command = CreateCommand();

            _userRepo.UserExistByEmailAsync(command.Email, Arg.Any<CancellationToken>())
                .Returns(false);
            _userRepo.UserExistByCpfAsync(Arg.Any<string>() , Arg.Any<CancellationToken>())
                .Returns(true);

            var result = await _handler.Handle(command, CancellationToken.None);

            result.IsT1.Should().BeTrue();
            var error = result.AsT1;
            error.Should().BeOfType<CpfAlreadyExistError>();
        }

        [Fact]
        [Trait("Features", "Register")]
        public async Task Handle_ShouldHashPassword_BeforeSaving()
        {
            var command = CreateValidSetup();

            await _handler.Handle(command, CancellationToken.None);

            _passwordHasher.Received(1).Hash(command.Password);
        }

        [Fact]
        [Trait("Features", "Register")]
        public async Task Handle_ShouldHashCpf_BeforeSaving()
        {
            var command = CreateValidSetup();

            await _handler.Handle(command, CancellationToken.None);

            _cpfHasher.Received(1).Hash(command.Cpf);
        }

        [Fact]
        [Trait("Features", "Register")]
        public async Task Handle_ShouldNormalizeEmail_BeforeSaving()
        {
            var command = CreateValidSetup();

            await _handler.Handle(command, CancellationToken.None);

            await _userRepo.Received(1).Add(Arg.Is<User>(u => u.Email == command.Email.Trim().ToLower()));
        }

        [Fact]
        [Trait("Features", "Register")]
        public async Task Handle_ShouldCommit_WhenUserIsCreatedSuccessfully()
        {
            var command = CreateValidSetup();

            await _handler.Handle(command, CancellationToken.None);

            await _uow.Received(1).CommitAsync(Arg.Any<CancellationToken>());
        }  

        private RegisterCommand CreateCommand()
        => new RegisterCommand(
            _faker.Name.FirstName().Replace(" ", ""),
            _faker.Name.LastName(),
            _faker.Internet.Email().Trim().ToLower(),
            _faker.Person.Cpf(false),
            _faker.Internet.Password(10)
        );

        private RegisterCommand CreateValidSetup()
        {
            var command = CreateCommand();

            _userRepo.UserExistByEmailAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);
            _userRepo.UserExistByCpfAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);

            return command;
        }
    } 
}