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

        public async Task<List<RefreshToken>> GetAllValidTokensByUserAsync(Guid userId, CancellationToken ct)
        => await _context.refreshTokens
                    .Where(rt => rt.UserId == userId && rt.IsRevoked == false)
                    .OrderBy(rt => rt.ExpiresAt)
                    .ToListAsync(ct);

        public async Task<RefreshToken?> GetRefreshTokenByTokenAsync(string token, CancellationToken ct)
        => await _context.refreshTokens
                    .Where(rt => rt.Token == token)
                    .FirstOrDefaultAsync(ct);
    }
}