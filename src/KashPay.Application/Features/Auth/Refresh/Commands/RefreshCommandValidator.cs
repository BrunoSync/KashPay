using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace KashPay.Application.Features.Auth.Refresh.Commands
{
    public class RefreshCommandValidator : AbstractValidator<RefreshCommand>
    {
        public RefreshCommandValidator()
        {
            RuleFor(rt => rt.RefreshToken)
                .NotEmpty().WithMessage("Refresh token can't be empty");
        }
    }
}