using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KashPay.Application.Common.Interfaces.Infrastructure;
using KashPay.Application.Common.Interfaces.Infrastructure.Repositories;
using KashPay.Application.Common.OneOf;
using KashPay.Application.Common.OneOf.Errors;
using KashPay.Application.Features.Auth.ForgetPassword.Commands;
using KashPay.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using OneOf;

namespace KashPay.Application.Features.Auth.ResetPassword.Commands
{ 
    public class ResetPasswordHandler : IRequestHandler<ResetPasswordCommand,OneOf<ResetPasswordResponse, AppError>>
    {
        private readonly IPasswordResetTokenRepository _token;
        private readonly IPasswordHasher _hasher;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<ResetPasswordHandler> _logger;

        public ResetPasswordHandler(IPasswordResetTokenRepository token, IPasswordHasher hasher, IUnitOfWork uow, ILogger<ResetPasswordHandler> logger)
        {
            _token = token;
            _hasher = hasher;
            _uow = uow;
            _logger = logger;
        }

        public async Task<OneOf<ResetPasswordResponse, AppError>> Handle(ResetPasswordCommand command, CancellationToken ct)
        {
            var normalizedCode = command.Token.Replace("-", "").Replace(" ", "");
            var token = await _token.GetPasswordResetTokenAsync(normalizedCode, command.Email, ct);

            if (token is null)
            {
                _logger.LogWarning("Password reset token not found");
                return new PasswordResetTokenNotFoundError();
            }

            if (token.Expiration < DateTime.UtcNow || token.IsRevoked)
            {
                _logger.LogWarning("Invalid password reset token");
                return new InvalidPasswordResetTokenError();
            }

            var newPassHashed = _hasher.Hash(command.NewPassword);

            token.User.ChangeHashPassword(newPassHashed);
            token.Revoke();
            await _uow.CommitAsync(ct);

            _logger.LogInformation("Password successful reseted, userId = {id}", token.User.Id);

            return new ResetPasswordResponse("Password successfully changed.");
        }
    }
}