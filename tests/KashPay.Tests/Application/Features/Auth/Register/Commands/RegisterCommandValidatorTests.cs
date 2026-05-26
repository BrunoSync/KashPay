using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using Bogus.Extensions.Brazil;
using FluentAssertions;
using FluentValidation;
using KashPay.Application.Features.Auth.Login.Register.Commands;
using NSubstitute;

namespace KashPay.Tests.Application.Features.Auth.Register.Commands
{
    public class RegisterCommandValidatorTests
    {
        private readonly IValidator<RegisterCommand> _validator;
        private readonly Faker _faker = new("pt_BR");

        public RegisterCommandValidatorTests()
        {
            _validator = new RegisterCommandValidation();
        }

        [Fact]
        [Trait("Features", "Register")]
        public void Validator_ShouldPass_WhenValidDataIsProvided()
        {
            var command = CreateCommand();

            var result = _validator.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        [Trait("Features", "Register")]
        public void Validator_ShouldFail_WhenFirstNameIsEmpty()
        {
            var command = CreateCommand(firstName: "");

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.PropertyName == "FirstName");
        }

        [Fact]
        [Trait("Features", "Register")]
        public void Validator_ShouldFail_WhenFirstNameContainsNumbers()
        {
            var command = CreateCommand(firstName: $"{_faker.Name.FirstName()}1");

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.PropertyName == "FirstName");
        }

        [Fact]
        [Trait("Features", "Register")]
        public void Validator_ShouldFail_WhenLastNameIsEmpty()
        {
            var command = CreateCommand(lastName: "");

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.PropertyName == "LastName");
        }

        [Fact]
        [Trait("Features", "Register")]
        public void Validator_ShouldFail_WhenLastNameContainsNumbers()
        {
            var command = CreateCommand(lastName: $"{_faker.Name.LastName()}1");

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.PropertyName == "LastName");
        }

        [Fact]
        [Trait("Features", "Register")]
        public void Validator_ShouldFail_WhenEmailIsInvalid()
        {
            var command = CreateCommand(email: "invalid.email");

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.PropertyName == "Email");
        }

        [Fact]
        [Trait("Features", "Register")]
        public void Validator_ShouldFail_WhenPasswordIsTooShort()
        {
            var command = CreateCommand(password: _faker.Internet.Password(5));

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.PropertyName == "Password");
        }

        [Fact]
        [Trait("Features", "Register")]
        public void Validator_ShouldFail_WhenPasswordIsTooLong()
        {
            var command = CreateCommand(password: _faker.Internet.Password(50));

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.PropertyName == "Password");
        }

        [Fact]
        [Trait("Features", "Register")]
        public void Validator_ShouldFail_WhenCpfIsInvalid()
        {
            var command = CreateCommand(cpf: "12345678900");

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.PropertyName == "Cpf");
        }

        private RegisterCommand CreateCommand(
        string? firstName = null,
        string? lastName = null,
        string? email = null,
        string? cpf = null,
        string? password = null) => new(
            firstName ?? _faker.Name.FirstName().Replace(" ", ""),
            lastName ?? _faker.Name.LastName().Replace(" ", ""),
            email ?? _faker.Internet.Email(),
            cpf ?? _faker.Person.Cpf(false),
            password ?? _faker.Internet.Password(10)
        );
    }
}