using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KashPay.Application.Common.Interfaces.Infrastructure.Repositories;
using KashPay.Domain.Entities;
using KashPay.Infrastructure.Data;

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
    }
}