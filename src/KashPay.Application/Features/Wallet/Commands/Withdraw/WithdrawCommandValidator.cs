using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace KashPay.Application.Features.Wallet.Commands.Withdraw
{
    public class WithdrawCommandValidator : AbstractValidator<WithdrawCommand>
    {
        public WithdrawCommandValidator()
        {
            RuleFor(a => a.Amount)
                .ExclusiveBetween(10, 50000).WithMessage("Amount should be between 10 and 50.000");
        }
    }
}