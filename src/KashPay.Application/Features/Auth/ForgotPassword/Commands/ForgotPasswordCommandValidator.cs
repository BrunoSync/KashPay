using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace KashPay.Application.Features.Auth.ForgotPassword.Commands
{
    public class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
    {
        public ForgotPasswordCommandValidator()
        {
            RuleFor(e => e.Email)
                .NotEmpty().WithMessage("Email can't be empty")
                .MaximumLength(254).WithMessage("EMAIL - Max: 254 characters")
                .EmailAddress().WithMessage("Invalid Email");
        }
    }
}