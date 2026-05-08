using System;
using System.Collections.Generic;
using System.Data.Common;
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

        public async Task<Wallet?> FindWalletLockByIdAsync(Guid walletId, CancellationToken ct)
        => await _context.wallets
                    .FromSqlRaw("SELECT * FROM wallets WHERE id = {0} FOR UPDATE", walletId)
                    .FirstOrDefaultAsync(ct);

        // === Queries ===
        public async Task<Guid?> FindWalletIdByUserIdAsync(Guid userId, CancellationToken ct)
        => await _context.wallets
                    .AsNoTracking()
                    .Where(w => w.UserId == userId)
                    .Select(w => w.Id)
                    .FirstOrDefaultAsync(ct);
        
        public async Task<Guid?> FindWalletIdByAccountNumberAsync(string accountNumber, CancellationToken ct)
        => await _context.wallets
                    .AsNoTracking()
                    .Where(w => w.AccountNumber == accountNumber)
                    .Select(w => w.Id)
                    .FirstOrDefaultAsync(ct);

        public async Task<Wallet?> GetWalletByUserIdAsync(Guid userId, CancellationToken ct)
        => await _context.wallets
                    .AsNoTracking()
                    .Where(w => w.UserId == userId)
                    .FirstOrDefaultAsync(ct);
    }
}