using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using KashPay.Application.Common.Interfaces.Infrastructure;
using KashPay.Application.Common.Interfaces.Infrastructure.Repositories;
using KashPay.Application.Common.Interfaces.Repositories;
using KashPay.Application.Common.OneOf;
using KashPay.Application.Common.OneOf.Errors;
using KashPay.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using OneOf;

namespace KashPay.Application.Features.Auth.ForgotPassword.Commands
{
    public class ForgotPasswordHandler : IRequestHandler<ForgotPasswordCommand, ForgotPasswordResponse>
    {
        private readonly IUserRepository _userRepo;
        private readonly IPasswordResetTokenRepository _tokenRepo;
        private readonly IEmailService _email;
        private readonly IUnitOfWork _uow;
        private readonly  ILogger<ForgotPasswordHandler> _logger;

        public ForgotPasswordHandler(IUserRepository userRepo, IPasswordResetTokenRepository tokenRepo, IEmailService email, IUnitOfWork uow,
            ILogger<ForgotPasswordHandler> logger 
        )
        {
            _userRepo = userRepo;
            _tokenRepo = tokenRepo;
            _email = email;
            _uow = uow;
            _logger = logger;
        }

        public async Task<ForgotPasswordResponse> Handle(ForgotPasswordCommand command, CancellationToken ct)
        {
            var user = await _userRepo.GetUserByCredentialsAsync(command.Email, ct);

            if (user is null)
                return new ForgotPasswordResponse("If the email exists, you will receive the instructions.");

            var code = $"{Random.Shared.Next(100, 999).ToString()}-{Random.Shared.Next(100, 999).ToString()}";
            var normalizedCode = code.Replace("-", "").Replace(" ", "");
            var newPasswordResetToken = new PasswordResetToken(
                user.Id,
                normalizedCode,
                DateTime.UtcNow.AddMinutes(30)
            );
            await _tokenRepo.AddAsync(newPasswordResetToken, ct);
            await _uow.CommitAsync(ct);

            await _email.SendPasswordResetEmailAsync(command.Email, code, ct);

            _logger.LogInformation("Password reset code sent: {id}", user.Id);

            return new ForgotPasswordResponse("If the email exists, you will receive the instructions.");
        }
    }
}