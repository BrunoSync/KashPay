using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace KashPay.Application.Features.Wallet.Queries.GetBalance
{
    public class GetBalanceQueryValidator : AbstractValidator<GetBalanceQuery>
    {
        public GetBalanceQueryValidator()
        {
            RuleFor(u => u.UserId)
                .NotEmpty().WithMessage("User id can't be empty");
        }
    }
}