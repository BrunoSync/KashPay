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
    public class TransactionRepository : ITransactionRepository
    {
        // Database
        private readonly AppDbContext _context;

        public TransactionRepository(AppDbContext context)
        {
            _context = context;
        }

        // === Commands ===
        public async Task Add(Transaction transaction)
        => _context.transactions.Add(transaction);

        // === Queries ===
        public async Task<List<Transaction>> GetByWalletIdAsync
        (Guid walletId, 
        int pageSize, 
       ( DateTime? timeStamp, Guid? id)? cursor, 
        CancellationToken ct)
        {
            var query =  _context.transactions
                            .AsNoTracking()
                            .Where(t => t.FromAccountId == walletId || t.ToAccountId == walletId);

            if (cursor.HasValue)
                query = query.Where(t => t.CreatedAt < cursor.Value.timeStamp
                    || (t.CreatedAt == cursor.Value.timeStamp && t.Id < cursor.Value.id)
                );

            return await query
                .OrderByDescending(t => t.CreatedAt)
                .Take(pageSize)
                .ToListAsync(ct);
        }
    }
}