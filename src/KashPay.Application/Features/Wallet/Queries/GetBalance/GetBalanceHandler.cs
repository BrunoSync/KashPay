using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KashPay.Application.Common.Interfaces.Infrastructure.Repositories;
using KashPay.Application.Common.OneOf;
using KashPay.Application.Common.OneOf.Errors;
using MediatR;
using Microsoft.Extensions.Logging;
using OneOf;

namespace KashPay.Application.Features.Wallet.Queries.GetBalance
{
    public class GetBalanceHandler : IRequestHandler<GetBalanceQuery, OneOf<GetBalanceResponse, AppError>>
    {
        private readonly IWalletRepository _walletRepo;
        private readonly ILogger<GetBalanceHandler> _logger;

        public GetBalanceHandler(IWalletRepository walletRepo, ILogger<GetBalanceHandler> logger)
        {
            _walletRepo = walletRepo;
            _logger = logger;
        }

        public async Task<OneOf<GetBalanceResponse, AppError>> Handle(GetBalanceQuery query, CancellationToken ct)
        {
            var wallet = await _walletRepo.GetWalletByUserIdAsync(query.UserId, ct);

            if (wallet is null)
            {
                _logger.LogWarning("Wallet not found");
                return new WalletNotFoundError();
            }

            return new GetBalanceResponse(wallet.Balance);
        }
    }
}