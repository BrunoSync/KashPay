using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace KashPay.Application.Features.Wallet.Commands.Deposit
{
    public class DepositCommandValidator : AbstractValidator<DepositCommand>
    {
        public DepositCommandValidator()
        {
            RuleFor(a => a.Amount)
                .ExclusiveBetween(10, 50000).WithMessage("Amount should be between 10 and 50.000");
        }
    }
}