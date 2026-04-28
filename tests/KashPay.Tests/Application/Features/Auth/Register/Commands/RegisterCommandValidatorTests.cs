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
            var command = new RegisterCommand(
                _faker.Name.FirstName().Replace(" ", ""),
                _faker.Name.LastName().Replace(" ", ""),
                _faker.Internet.Email(),
                _faker.Person.Cpf(false),
                _faker.Internet.Password(10)
            );

            var result = _validator.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        [Trait("Features", "Register")]
        public void Validator_ShouldFail_WhenFirstNameIsEmpty()
        {
            var command = new RegisterCommand(
                "",
                _faker.Name.LastName().Replace(" ", ""),
                _faker.Internet.Email(),
                _faker.Person.Cpf(false),
                _faker.Internet.Password(10)
            );

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.PropertyName == "FirstName");
        }

        [Fact]
        [Trait("Features", "Register")]
        public void Validator_ShouldFail_WhenFirstNameContainsNumbers()
        {
            var command = new RegisterCommand(
                $"{_faker.Name.FirstName()}1",
                _faker.Name.LastName().Replace(" ", ""),
                _faker.Internet.Email(),
                _faker.Person.Cpf(false),
                _faker.Internet.Password(7)
            );

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.PropertyName == "FirstName");
        }

        [Fact]
        [Trait("Features", "Register")]
        public void Validator_ShouldFail_WhenLastNameIsEmpty()
        {
            var command = new RegisterCommand(
                _faker.Name.FirstName().Replace(" ", ""),
                "",
                _faker.Internet.Email(),
                _faker.Person.Cpf(false),
                _faker.Internet.Password(10)
            );

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.PropertyName == "LastName");
        }

        [Fact]
        [Trait("Features", "Register")]
        public void Validator_ShouldFail_WhenLastNameContainsNumbers()
        {
            var command = new RegisterCommand(
                _faker.Name.FirstName().Replace(" ", ""),
                $"{_faker.Name.LastName()}1",
                _faker.Internet.Email(),
                _faker.Person.Cpf(false),
                _faker.Internet.Password(10)
            );

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.PropertyName == "LastName");
        }

        [Fact]
        [Trait("Features", "Register")]
        public void Validator_ShouldFail_WhenEmailIsInvalid()
        {
            var command = new RegisterCommand(
                _faker.Name.FirstName().Replace(" ", ""),
                _faker.Name.LastName().Replace(" ", ""),
                "email.invalid",
                _faker.Person.Cpf(false),
                _faker.Internet.Password(10)
            );

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.PropertyName == "Email");
        }

        [Fact]
        [Trait("Features", "Register")]
        public void Validator_ShouldFail_WhenPasswordIsTooShort()
        {
            var command = new RegisterCommand(
                _faker.Name.FirstName().Replace(" ", ""),
                _faker.Name.LastName().Replace(" ", ""),
                _faker.Internet.Email(),
                _faker.Person.Cpf(false),
                _faker.Internet.Password(7)
            );

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.PropertyName == "Password");
        }

        [Fact]
        [Trait("Features", "Register")]
        public void Validator_ShouldFail_WhenPasswordIsTooLong()
        {
            var command = new RegisterCommand(
                _faker.Name.FirstName().Replace(" ", ""),
                _faker.Name.LastName().Replace(" ", ""),
                _faker.Internet.Email(),
                _faker.Person.Cpf(false),
                _faker.Internet.Password(33)
            );

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.PropertyName == "Password");
        }

        [Fact]
        [Trait("Features", "Register")]
        public void Validator_ShouldFail_WhenCpfIsInvalid()
        {
            var command = new RegisterCommand(
                _faker.Name.FirstName().Replace(" ", ""),
                _faker.Name.LastName().Replace(" ", ""),
                _faker.Internet.Email(),
                "12345678900",
                _faker.Internet.Password(10)
            );

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle(e => e.PropertyName == "Cpf");
        }
    }
}