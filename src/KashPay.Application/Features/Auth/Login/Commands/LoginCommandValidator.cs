using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace KashPay.Application.Features.Auth.Login.Queries
{
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(c => c.Credentials)
                .MinimumLength(11).WithMessage("CREDENTIALS - Min: 11 characters")
                .MaximumLength(254).WithMessage("CREDENTIALS - Max: 254 characters");

            RuleFor(p => p.Password)
                .NotEmpty().WithMessage("Password can't be empty");
        }
    }
}