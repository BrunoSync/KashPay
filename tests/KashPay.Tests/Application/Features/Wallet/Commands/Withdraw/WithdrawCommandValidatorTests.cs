using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using KashPay.Application.Features.Wallet.Commands.Withdraw;

namespace KashPay.Tests.Application.Features.Wallet.Commands.Withdraw
{
    public class WithdrawCommandValidatorTests
    {
        private readonly IValidator<WithdrawCommand> _validator;

        public WithdrawCommandValidatorTests()
        {
            _validator = new WithdrawCommandValidator();
        }

        [Fact]
        [Trait("Features", "Withdraw")]
        public void Validator_ShouldPass_WhenAmountIsValid()
        {
            var amount = Random.Shared.Next(10, 50000);
            var command = new WithdrawCommand(
                Guid.NewGuid(),
                amount
            );

            var result = _validator.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        [Trait("Features", "Withdraw")]
        public void Validator_ShouldFail_WhenAmountIsTooLow()
        {
            var amount = Random.Shared.Next(1, 9);
            var command = new WithdrawCommand(
                Guid.NewGuid(),
                amount
            );

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Amount");
        }

        [Fact]
        [Trait("Features", "Withdraw")]
        public void Validator_ShouldFail_WhenAmountIsTooHigh()
        {
            var amount = Random.Shared.Next(50000, 51000);
            var command = new WithdrawCommand(
                Guid.NewGuid(),
                amount
            );

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Amount");
        }
    }
}