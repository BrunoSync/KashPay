using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KashPay.Domain.Entities;

namespace KashPay.Application.Common.Interfaces.Infrastructure.Repositories
{
    public interface IWalletRepository
    {
        // === Commands ===
        Task Add(Wallet wallet);
        Task<Wallet?> FindWalletLockByIdAsync(Guid walletId, CancellationToken ct);

        // === Queries ===
        Task<Guid?> FindWalletIdByUserIdAsync(Guid userId, CancellationToken ct);
        Task<Guid?> FindWalletIdByAccountNumberAsync(string accountNumber, CancellationToken ct);
        Task<Wallet?> GetWalletByUserIdAsync(Guid userId, CancellationToken ct);
    }
}