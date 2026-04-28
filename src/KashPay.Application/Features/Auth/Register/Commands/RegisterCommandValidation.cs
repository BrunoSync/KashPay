using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using KashPay.Application.Common.Interfaces.Infrastructure;

namespace KashPay.Application.Features.Auth.Login.Register.Commands
{
    public class RegisterCommandValidation : AbstractValidator<RegisterCommand>
    {
        public RegisterCommandValidation()
        {
            RuleFor(fn => fn.FirstName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("First name can't be empty")
                .MaximumLength(50).WithMessage("FIRST NAME - Max: 50 characters")
                .Matches(@"^[a-zA-ZÀ-ÿ]+$").WithMessage("Must contain only letters");

            RuleFor(ln => ln.LastName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Last name can't be empty")
                .MaximumLength(100).WithMessage("LAST NAME - Max: 100 characters")
                .Matches(@"^[a-zA-ZÀ-ÿ]+$").WithMessage("Must contain only letters");

            RuleFor(e => e.Email)
                .NotEmpty().WithMessage("Email can't be empty")
                .MaximumLength(254).WithMessage("EMAIL - Max: 254 characters")
                .EmailAddress().WithMessage("Invalid Email");

            RuleFor(c => c.Cpf)
                .MinimumLength(11).WithMessage("Invalid CPF")
                .MaximumLength(11).WithMessage("Invalid CPF")
                .Must(BeAValidCpf).WithMessage("Invalid CPF");

            RuleFor(p => p.Password)
                .MinimumLength(8).WithMessage("PASSWORD - Min: 8 characters")
                .MaximumLength(32).WithMessage("PASSWORD - Max: 32 characters");
        }

         private bool BeAValidCpf(string cpf)
        {
            var digits = new string(cpf.Where(char.IsDigit).ToArray());

            if (digits.Length != 11) return false;
            if (digits.Distinct().Count() == 1) return false;

            var sum = 0;
            for (int i = 0; i < 9; i++)
                sum += int.Parse(digits[i].ToString()) * (10 - i);

            var remainder = sum % 11;
            var digit1 = remainder < 2 ? 0 : 11 - remainder;
            if (digit1 != int.Parse(digits[9].ToString())) return false;

            sum = 0;
            for (int i = 0; i < 10; i++)
                sum += int.Parse(digits[i].ToString()) * (11 - i);

            remainder = sum % 11;
            var digit2 = remainder < 2 ? 0 : 11 - remainder;
            return digit2 == int.Parse(digits[10].ToString());
        }
    }
}