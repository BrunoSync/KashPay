using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using KashPay.Application.Features.Transaction.Queries.GetTransactions;

namespace KashPay.Tests.Application.Features.Transaction.Queries.GetTransactions
{
    public class GetTransactionsQueryValidatorTests
    {
        private readonly IValidator<GetTransactionsQuery> _validator;

        public GetTransactionsQueryValidatorTests()
        {
            _validator = new GetTransactionsQueryValidator();
        }

        [Fact]
        [Trait("Features", "GetTransactions")]
        public void Validator_ShouldPass_WhenPageSizeIsValid()
        {
            var query = new GetTransactionsQuery(Guid.NewGuid(), 10, null);

            var result = _validator.Validate(query);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        [Trait("Features", "GetTransactions")]
        public void Validator_ShouldFail_WhenPageSizeIsTooLow()
        {
            var query = new GetTransactionsQuery(Guid.NewGuid(), 0, null);

            var result = _validator.Validate(query);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "PageSize");
        }

        [Fact]
        [Trait("Features", "GetTransactions")]
        public void Validator_ShouldFail_WhenPageSizeIsTooHigh()
        {
            var query = new GetTransactionsQuery(Guid.NewGuid(), 51, null);

            var result = _validator.Validate(query);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "PageSize");
        }
    }
}