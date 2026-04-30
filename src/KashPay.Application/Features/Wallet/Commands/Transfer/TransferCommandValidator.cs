using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace KashPay.Application.Features.Wallet.Commands.Transfer
{
    public class TransferCommandValidator : AbstractValidator<TransferCommand>
    {
        public TransferCommandValidator()
        {
            RuleFor(a => a.AccountNumber)
                .NotEmpty().WithMessage("Account number can't be empty");

            RuleFor(x => x.Amount)
                .ExclusiveBetween(9, 100001).WithMessage("Amount should be between 10 and 100.000");
        }
    }
}