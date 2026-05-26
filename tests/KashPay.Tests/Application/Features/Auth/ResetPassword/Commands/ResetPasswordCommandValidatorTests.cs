using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using FluentValidation;
using KashPay.Application.Features.Auth.ForgetPassword.Commands;

namespace KashPay.Tests.Application.Features.Auth.ResetPassword.Commands
{
    public class ResetPasswordCommandValidatorTests
    {
        private readonly IValidator<ResetPasswordCommand> _validator;
        private readonly Faker _faker = new("pt_BR");

        public ResetPasswordCommandValidatorTests()
        {
            _validator = new ResetPasswordCommandValidator();
        }

        [Fact]
        [Trait("Features", "ResetPassword")]
        public void Validator_ShouldPass_WhenValidDataIsProvided()
        {
            var command = CreateCommand();

            var result = _validator.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        [Trait("Features", "ResetPassword")]
        public void Validator_ShouldFail_WhenEmailIsEmpty()
        {
            var command = CreateCommand(email: "");

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Email");
        }

        [Fact]
        [Trait("Features", "ResetPassword")]
        public void Validator_ShouldFail_WhenEmailIsTooLong()
        {
            var email = new string('a', 255);
            var command = CreateCommand(email: email);

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Email");
        }

        [Fact]
        [Trait("Features", "ResetPassword")]
        public void Validator_ShouldFail_WhenEmailIsInvalid()
        {
            var command = CreateCommand(email: "email.com");

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Email");
        }

        [Fact]
        [Trait("Features", "ResetPassword")]
        public void Validator_ShouldFail_WhenTokenIsEmpty()
        {
            var command = CreateCommand(token: "");

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Token");
        }

        [Fact]
        [Trait("Features", "ResetPassword")]
        public void Validator_ShouldFail_WhenPasswordIsTooShort()
        {
            var command = CreateCommand(newPassword: _faker.Internet.Password(5));

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "NewPassword");
        }

        [Fact]
        [Trait("Features", "ResetPassword")]
        public void Validator_ShouldFail_WhenPasswordIsTooLong()
        {
            var command = CreateCommand(newPassword: _faker.Internet.Password(33));

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "NewPassword");
        }

        [Fact]
        [Trait("Features", "ResetPassword")]
        public void Validator_ShouldFail_WhenPasswordsDoNotMatch()
        {
            var command = CreateCommand(ConfirmNewPassword: "wrongpass");

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "ConfirmNewPassword");
        }

        private ResetPasswordCommand CreateCommand(string? email = null,
            string? token = null,
            string? newPassword = null,
            string? ConfirmNewPassword = null)
        {
            var password = _faker.Internet.Password(10);
            return new ResetPasswordCommand(
                email ?? _faker.Internet.Email(),
                token ?? "123-456",
                newPassword ?? password,
                ConfirmNewPassword ?? password
            );
        }
    }
}