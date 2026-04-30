using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using KashPay.Application.Features.Wallet.Commands.Deposit;

namespace KashPay.Tests.Application.Features.Wallet.Commands.Deposit
{
    public class DepositCommandValidatorTests
    {
        private readonly IValidator<DepositCommand> _validator;

        public DepositCommandValidatorTests()
        {
            _validator = new DepositCommandValidator();
        }

        [Fact]
        [Trait("Features", "Deposit")]
        public void Validator_ShouldPass_WhenAmountIsValid()
        {
            var amount = Random.Shared.Next(10, 50000);
            var command = new DepositCommand(
                Guid.NewGuid(),
                amount
            );

            var result = _validator.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        [Trait("Features", "Deposit")]
        public void Validator_ShouldFail_WhenAmountIsTooLow()
        {
            var amount = Random.Shared.Next(1, 9);
            var command = new DepositCommand(
                Guid.NewGuid(),
                amount
            );

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Amount");
        }

        [Fact]
        [Trait("Features", "Deposit")]
        public void Validator_ShouldFail_WhenAmountIsTooHigh()
        {
            var amount = Random.Shared.Next(50000, 51000);
            var command = new DepositCommand(
                Guid.NewGuid(),
                amount
            );

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Amount");
        }
    }
}