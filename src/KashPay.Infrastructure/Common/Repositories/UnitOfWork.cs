using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KashPay.Application.Common.Interfaces.Infrastructure.Repositories;
using KashPay.Infrastructure.Data;

namespace KashPay.Infrastructure.Common.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        // Database
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public async Task CommitAsync(CancellationToken ct)
        => await _context.SaveChangesAsync(ct);
    }
}