using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KashPay.Application.Common.Interfaces.Infrastructure.Repositories;
using KashPay.Domain.Entities;
using KashPay.Infrastructure.Data;

namespace KashPay.Infrastructure.Common.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        // Database 
        private readonly AppDbContext _context;

        public RefreshTokenRepository(AppDbContext context)
        {
            _context = context;
        }

        // === Commands ===
        public async Task Add(RefreshToken refreshToken)
        => _context.refreshTokens.Add(refreshToken);
    }
}