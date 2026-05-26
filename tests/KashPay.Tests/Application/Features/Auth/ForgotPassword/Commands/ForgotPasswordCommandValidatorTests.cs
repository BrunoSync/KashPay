using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using FluentValidation;
using KashPay.Application.Features.Auth.ForgotPassword.Commands;

namespace KashPay.Tests.Application.Features.Auth.ForgotPassword.Commands
{
    public class ForgotPasswordCommandValidatorTests
    {
        private readonly IValidator<ForgotPasswordCommand> _validator;
        private readonly Faker _faker = new("pt_BR");

        public ForgotPasswordCommandValidatorTests()
        {
            _validator = new ForgotPasswordCommandValidator();
        }

        [Fact]
        [Trait("Features", "ForgotPassword")]
        public void Validator_ShouldReturnSucess_WhenEmailIsValid()
        {
            var command = new ForgotPasswordCommand(_faker.Internet.Email());

            var result = _validator.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        [Trait("Features", "ForgotPassword")]
        public void Validator_ShouldReturnFail_WhenEmailIsEmpty()
        {
            var command = new ForgotPasswordCommand("");

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Email");
        }

        [Fact]
        [Trait("Features", "ForgotPassword")]
        public void Validator_ShouldReturnFail_WhenEmailIsTooLong()
        {
            var command = new ForgotPasswordCommand(new string('a', 255));

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Email");
        }

        [Fact]
        [Trait("Features", "ForgotPassword")]
        public void Validator_ShouldReturnFail_WhenEmailIsInvalid()
        {
            var command = new ForgotPasswordCommand($"{_faker.Person.FirstName}.com");

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Email");
        }
    }
}