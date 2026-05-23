using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace KashPay.Application.Features.Auth.ForgetPassword.Commands
{
    public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordCommandValidator()
        {
            RuleFor(e => e.Email)
                .NotEmpty().WithMessage("Email can't be empty")
                .MaximumLength(254).WithMessage("EMAIL - Max: 254 characters")
                .EmailAddress().WithMessage("Invalid Email");

            RuleFor(t => t.Token)
                .NotEmpty().WithMessage("Token can't be empty");

            RuleFor(p => p.NewPassword)
                .MinimumLength(8).WithMessage("PASSWORD - Min: 8 characters")
                .MaximumLength(32).WithMessage("PASSWORD - Max: 32 characters");

            RuleFor(cnp => cnp.ConfirmNewPassword)
                .Matches(p => p.NewPassword).WithMessage("Both passwords must be the same");
        }
    }
}