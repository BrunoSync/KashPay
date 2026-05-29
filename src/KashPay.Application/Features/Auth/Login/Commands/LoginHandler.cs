using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using FluentValidation;
using KashPay.Application.Common.Interfaces.Infrastructure;
using KashPay.Application.Common.Interfaces.Infrastructure.Repositories;
using KashPay.Application.Common.Interfaces.Jwt;
using KashPay.Application.Common.Interfaces.Repositories;
using KashPay.Application.Common.OneOf;
using KashPay.Application.Common.OneOf.Errors;
using KashPay.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using OneOf;

namespace KashPay.Application.Features.Auth.Login.Queries
{
    public class LoginHandler : IRequestHandler<LoginCommand, OneOf<LoginResponse, AppError>>
    {
        private readonly ICpfHasher _cpfHasher;
        private readonly IUnitOfWork _uow;
        private readonly IPasswordHasher _passHasher;
        private readonly IJwtService _token;
        private readonly IUserRepository _userRepo;
        private readonly IRefreshTokenRepository _rtRepo;
        private readonly ILogger<LoginHandler> _logger;

        public LoginHandler(ICpfHasher cpfHasher, IUnitOfWork uow, IPasswordHasher passHasher, IJwtService token, 
            IUserRepository userRepo, IRefreshTokenRepository rtRepo, ILogger<LoginHandler> logger
            )
        {
            _cpfHasher = cpfHasher;
            _uow = uow;
            _passHasher = passHasher;
            _token = token;
            _userRepo = userRepo;
            _rtRepo = rtRepo;
            _logger = logger;
        }

         public async Task<OneOf<LoginResponse, AppError>> Handle(LoginCommand command, CancellationToken ct)
         {  
            var normalizedCredential = string.Empty;

            if (!command.Credentials.Contains("@"))
            {
                // Normalize and hash cpf
                var normalizedCpf = _cpfHasher.Hash(command.Credentials);
                normalizedCredential = normalizedCpf;
            }
            else
            {
                normalizedCredential = command.Credentials.Trim().ToLower();
            }

            var user = await _userRepo.GetUserByCredentialsAsync(normalizedCredential, ct);

            if (user is null || !_passHasher.Validate(command.Password, user.HashPassword))
            {
                _logger.LogWarning("Login failed because invalid credentials, Credential = {email}", command.Credentials);
                return new InvalidCredentialsError();
            }

            var accessToken = _token.GenerateAccessToken(user);
            var (rt, expiration) = _token.GenerateRefreshToken();

            var refreshToken = new RefreshToken(
                user.Id,
                rt,
                DateTime.UtcNow.AddDays(expiration)
            );

            await _rtRepo.Add(refreshToken);
            await _uow.CommitAsync(ct);

            _logger.LogInformation("Login successful: UserId = {userId} | UserEmail = {userEmail}", user.Id, user.Email);

            return new LoginResponse(
                accessToken,
                rt,
                DateTime.UtcNow
            );

        }
    }
}