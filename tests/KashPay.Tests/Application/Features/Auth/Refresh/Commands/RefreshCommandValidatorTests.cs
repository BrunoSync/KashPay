using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using KashPay.Application.Features.Auth.Refresh.Commands;

namespace KashPay.Tests.Application.Features.Auth.Refresh.Commands
{
    public class RefreshCommandValidatorTests
    {
        private readonly IValidator<RefreshCommand> _validator;

        public RefreshCommandValidatorTests()
        {
            _validator = new RefreshCommandValidator();
        }

        [Fact]
        [Trait("Features", "Refresh")]
        public void Validator_ShouldPass_WhenRefreshTokenIsProvided()
        {
            var command = new RefreshCommand(Guid.NewGuid().ToString());

            var result = _validator.Validate(command);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        [Trait("Features", "Refresh")]
        public void Validator_ShouldFail_WhenRefreshTokenIsEmpty()
        {
            var command = new RefreshCommand("");

            var result = _validator.Validate(command);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "RefreshToken");
        }
    }
}