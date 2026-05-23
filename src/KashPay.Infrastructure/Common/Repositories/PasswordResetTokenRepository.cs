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
    public class PasswordResetTokenRepository : IPasswordResetTokenRepository
    {
        private readonly AppDbContext _context;

        public PasswordResetTokenRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(PasswordResetToken token, CancellationToken ct)
        => await _context.passwordResetTokens.AddAsync(token, ct);

        public async Task<PasswordResetToken?> GetPasswordResetTokenAsync(string token, string email, CancellationToken ct)
        => await _context.passwordResetTokens
                    .Where(t => t.Token == token && t.User.Email == email)
                    .Include(u => u.User)
                    .FirstOrDefaultAsync(ct);
    }
}