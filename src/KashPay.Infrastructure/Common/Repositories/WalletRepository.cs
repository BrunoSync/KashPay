using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KashPay.Application.Common.Interfaces.Infrastructure.Repositories;
using KashPay.Domain.Entities;
using KashPay.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace KashPay.Infrastructure.Common.Repositories
{
    public class WalletRepository : IWalletRepository
    {
        // Database
        private readonly AppDbContext _context;

        public WalletRepository(AppDbContext context)
        {
            _context = context;
        }

        // === Commands ===
        public async Task Add(Wallet wallet)
        => _context.wallets.Add(wallet);

        public async Task<Wallet?> FindWalletByUserIdAsync(Guid userId, CancellationToken ct)
        => await _context.wallets
                    .Where(w => w.UserId == userId)
                    .FirstOrDefaultAsync(ct);
        
        public async Task<Wallet?> FindWalletByAccountNumberAsync(string accountNumber, CancellationToken ct)
        => await _context.wallets
                    .Where(w => w.AccountNumber == accountNumber)
                    .FirstOrDefaultAsync(ct);

        // === Queries ===
        public async Task<Wallet?> GetWalletByUserIdAsync(Guid userId, CancellationToken ct)
        => await _context.wallets
                    .AsNoTracking()
                    .Where(w => w.UserId == userId)
                    .FirstOrDefaultAsync(ct);
    }
}