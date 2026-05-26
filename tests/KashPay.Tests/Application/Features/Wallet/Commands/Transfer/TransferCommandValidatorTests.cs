using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using KashPay.Application.Features.Wallet.Commands.Transfer;

namespace KashPay.Tests.Application.Features.Wallet.Commands.Transfer
{
    public class TransferCommandValidatorTests
    {
        private readonly IValidator<TransferCommand> _validator;

        public TransferCommandValidatorTests()
        {
            _validator = new TransferCommandValidator();
        }

        [Fact]
        [Trait("Features", "Transfer")]
        public void Validator_ShouldPass_WhenValidDataIsProvided()
        {
            var command = CreateCommand();

            var result = _validator.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        [Trait("Features", "Transfer")]
        public void Validator_ShouldFail_WhenAccountNumberIsEmpty()
        {
            var command = CreateCommand(accountNumber: "");

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "AccountNumber");
        }

        [Fact]
        [Trait("Features", "Transfer")]
        public void Validator_ShouldFail_WhenAmountIsTooLow()
        {
            var command = CreateCommand(amount: 1);

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Amount");
        }

        [Fact]
        [Trait("Features", "Transfer")]
        public void Validator_ShouldFail_WhenAmountIsTooHigh()
        {
            var command = CreateCommand(amount: 100001);

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Amount");
        }

        private TransferCommand CreateCommand(
            string accountNumber = "123456-00",
            decimal amount = 1000) => 
            new(Guid.NewGuid(), accountNumber, amount);
    }
}