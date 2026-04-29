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
        Task<Wallet?> FindWalletByUserIdAsync(Guid userId, CancellationToken ct);
        Task<Wallet?> FindWalletByAccountNumberAsync(string accountNumber, CancellationToken ct);

        // === Queries ===
        Task<Wallet?> GetWalletByUserIdAsync(Guid userId, CancellationToken ct);
    }
}