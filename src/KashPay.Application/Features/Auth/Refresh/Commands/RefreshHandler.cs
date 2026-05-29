using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KashPay.Application.Common.Interfaces.Infrastructure.Repositories;
using KashPay.Application.Common.Interfaces.Jwt;
using KashPay.Application.Common.Interfaces.Repositories;
using KashPay.Application.Common.OneOf;
using KashPay.Application.Common.OneOf.Errors;
using KashPay.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using OneOf;

namespace KashPay.Application.Features.Auth.Refresh.Commands
{
    public class RefreshHandler : IRequestHandler<RefreshCommand, OneOf<RefreshResponse, AppError>>
    {
        private readonly IRefreshTokenRepository _rtRepo;
        private readonly IUserRepository _userRepo;
        private readonly IJwtService _token;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<RefreshHandler> _logger;

        public RefreshHandler(IRefreshTokenRepository rtRepo,IUserRepository userRepo,IJwtService token, IUnitOfWork uow, ILogger<RefreshHandler> logger)
        {
            _rtRepo = rtRepo;
            _userRepo = userRepo;
            _token = token;
            _uow = uow;
            _logger = logger;
        }

        public async Task<OneOf<RefreshResponse, AppError>> Handle(RefreshCommand command, CancellationToken ct)
        {
            var token = await _rtRepo.GetRefreshTokenByTokenAsync(command.RefreshToken, ct);

            if (token is null)
            {
                _logger.LogWarning("Refresh token not found");
                return new RefreshTokenNotFoundError();
            }

            if (token.ExpiresAt < DateTime.UtcNow || token.IsRevoked)
            {
                _logger.LogWarning("Refresh token expired");
                return new InvalidRefreshTokenError();
            }

            token.Revoke();

            var user = await _userRepo.GetUserByIdAsync(token.UserId, ct);

            if (user is null)
            {
                _logger.LogWarning("User not found, id = {id}", token.UserId);
                return new UserNotFoundError();
            }

            var newAccessToken = _token.GenerateAccessToken(user);
            var (newRt, expires) = _token.GenerateRefreshToken();

            var refreshToken = new RefreshToken(
                user.Id,
                newRt,
                DateTime.UtcNow.AddDays(expires)
            );

            await _rtRepo.Add(refreshToken);
            await _uow.CommitAsync(ct);

            _logger.LogInformation("Refresh token successful renoved, userid = {id}", user.Id);

            return new RefreshResponse(
                newAccessToken,
                newRt
            );
        }
    }
}