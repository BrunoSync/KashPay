using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KashPay.Application.Common.Interfaces.Infrastructure.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace KashPay.Application.Features.Auth.Logout.Commands
{
    public class LogoutHandler : IRequestHandler<LogoutCommand>
    {
        private readonly IRefreshTokenRepository _rtRepo;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<LogoutHandler> _logger;

        public LogoutHandler(IRefreshTokenRepository rtRepo, IUnitOfWork uow, ILogger<LogoutHandler> logger)
        {
            _rtRepo = rtRepo;
            _uow = uow;
            _logger = logger;
        }

        public async Task Handle(LogoutCommand command, CancellationToken ct)
        {
            var tokens = await _rtRepo.GetAllValidTokensByUserAsync(command.UserId, ct);

            tokens.ForEach(t => t.Revoke());

            _logger.LogInformation("All refresh tokens are revoked, User = {id}", command.UserId);

            await _uow.CommitAsync(ct);
        }
    }
}