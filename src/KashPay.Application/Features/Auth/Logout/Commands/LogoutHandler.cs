using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KashPay.Application.Common.Interfaces.Infrastructure.Repositories;
using MediatR;

namespace KashPay.Application.Features.Auth.Logout.Commands
{
    public class LogoutHandler : IRequestHandler<LogoutCommand>
    {
        private readonly IRefreshTokenRepository _rtRepo;
        private readonly IUnitOfWork _uow;

        public LogoutHandler(IRefreshTokenRepository rtRepo, IUnitOfWork uow)
        {
            _rtRepo = rtRepo;
            _uow = uow;
        }

        public async Task Handle(LogoutCommand command, CancellationToken ct)
        {
            var tokens = await _rtRepo.GetAllValidTokensByUserAsync(command.UserId, ct);

            tokens.ForEach(t => t.Revoke());

            await _uow.CommitAsync(ct);
        }
    }
}