using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Threading.Tasks;
using KashPay.Application.Common.Interfaces.Repositories;
using KashPay.Domain.Entities;
using KashPay.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace KashPay.Infrastructure.Common.Repositories
{
    public class UserRepository : IUserRepository
    {
        // Database
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        // === Commands ===
        public async Task Add(User user)
        => _context.users.Add(user);

        // === Queries ===
        public async Task<User?> GetUserByCredentialsAsync(string credential, CancellationToken ct)
        => await _context.users
                    .AsNoTracking()
                    .Where(u => u.Email == credential || u.HashCpf == credential)
                    .FirstOrDefaultAsync(ct);

        public async Task<User?> GetUserByIdAsync(Guid userId, CancellationToken ct)
        => await _context.users
                    .AsNoTracking()
                    .Where(u => u.Id == userId)
                    .FirstOrDefaultAsync(ct);

        public async Task<bool> UserExistByEmailAsync(string email, CancellationToken ct)
        => await _context.users
                    .AsNoTracking()
                    .AnyAsync(x => x.Email == email);

        public async Task<bool> UserExistByCpfAsync(string cpf, CancellationToken ct)
        => await _context.users
                    .AsNoTracking()
                    .AnyAsync(x => x.HashCpf == cpf);
    }
}