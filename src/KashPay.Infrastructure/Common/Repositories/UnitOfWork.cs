using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KashPay.Application.Common.Interfaces.Infrastructure.Repositories;
using KashPay.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.VisualBasic;

namespace KashPay.Infrastructure.Common.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        // Database
        private readonly AppDbContext _context;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public async Task BeginTransactionAsync(CancellationToken ct)
        => _transaction = await _context.Database.BeginTransactionAsync(ct);

        public async Task CommitAsync(CancellationToken ct)
        {
            await _context.SaveChangesAsync(ct);
            if (_transaction is null)
                throw new InvalidOperationException("No transaction in progress.");

            await _transaction.CommitAsync(ct);
        }
    }
}