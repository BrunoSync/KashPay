using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KashPay.Application.Common.Interfaces.Infrastructure;
using KashPay.Application.Common.Interfaces.Infrastructure.Repositories;
using KashPay.Application.Common.Interfaces.Repositories;
using KashPay.Application.Common.OneOf;
using KashPay.Application.Common.OneOf.Errors;
using KashPay.Domain.Entities;
using MediatR;
using OneOf;

namespace KashPay.Application.Features.Auth.Login.Register.Commands
{
    public class RegisterHandler : IRequestHandler<RegisterCommand, OneOf<RegisterResponse, AppError>>
    {
        private readonly IUserRepository _userRepo;
        private readonly IWalletRepository _walletRepo;
        private readonly IUnitOfWork _uow;
        private readonly ICpfHasher _cpfHasher;
        private readonly IPasswordHasher _passwordHasher;

        public RegisterHandler(IUserRepository userRepo, IWalletRepository walletRepo, IUnitOfWork uow, ICpfHasher cpfHasher, IPasswordHasher passwordHasher)
        {
            _userRepo = userRepo;
            _walletRepo = walletRepo;
            _uow = uow;
            _cpfHasher = cpfHasher;
            _passwordHasher = passwordHasher;
        }
        public async Task<OneOf<RegisterResponse, AppError>> Handle(RegisterCommand command, CancellationToken ct)
        {
            var emailNormalized = command.Email.Trim().ToLower();
            var emailExist = await _userRepo.UserExistByEmailAsync(emailNormalized, ct);

            if (emailExist)
                return new EmailAlreadyExistError();

            var hashedCpf = _cpfHasher.Hash(command.Cpf);
            var cpfExist = await _userRepo.UserExistByCpfAsync(hashedCpf, ct);

            if (cpfExist)
                return new CpfAlreadyExistError();

            var hashedPassword = _passwordHasher.Hash(command.Password);

            var newUser = new User(
                command.FirstName,
                command.LastName,
                emailNormalized,
                hashedCpf,
                hashedPassword
            );

            var newWallet = new Domain.Entities.Wallet(
                newUser.Id
            );

            await _userRepo.Add(newUser);
            await _walletRepo.Add(newWallet);
            await _uow.CommitAsync(ct);

            return new RegisterResponse(
                newUser.Id
            );
        }
    }
}