using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using Bogus.Extensions.Brazil;
using FluentAssertions;
using FluentValidation;
using KashPay.Application.Features.Auth.Login.Queries;

namespace KashPay.Tests.Application.Features.Auth.Login.Commands
{
    public class LoginCommandValidatorTests
    {
        private readonly IValidator<LoginCommand> _validator;
        private readonly Faker _faker = new("pt_BR");

        public LoginCommandValidatorTests()
        {
            _validator = new LoginCommandValidator();
        }

        [Fact]
        [Trait("Features", "Login")]
        public void Validator_ShouldPass_WhenCredentialsIsEmail()
        {
            var command = new LoginCommand(
                _faker.Internet.Email(),
                _faker.Internet.Password(10)
            );

            var result = _validator.Validate(command);
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        [Trait("Features", "Login")]
        public void Validator_ShouldPass_WhenCredentialsIsCpf()
        {
            var command = new LoginCommand(
                _faker.Person.Cpf(false),
                _faker.Internet.Password(10)
            );

            var result = _validator.Validate(command);
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        [Trait("Features", "Login")]
        public void Validator_ShouldFail_WhenCredentialsIsTooShort()
        {
            var command = new LoginCommand(
                new string('a', 10),
                _faker.Internet.Password(10)
            );

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Credentials");
        }

        [Fact]
        [Trait("Features", "Login")]
        public void Validator_ShouldFail_WhenCredentialsIsTooLong()
        {
            var command = new LoginCommand(
                new string('a', 255),
                _faker.Internet.Password(10)
            );

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Credentials");
        }

        [Fact]
        [Trait("Features", "Login")]
        public void Validator_ShouldFail_WhenPasswordIsEmpty()
        {
            var command = new LoginCommand(
                _faker.Person.Cpf(false),
                ""
            );

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Password");
        }
    }
}