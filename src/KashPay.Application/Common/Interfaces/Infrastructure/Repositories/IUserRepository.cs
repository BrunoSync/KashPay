using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KashPay.Domain.Entities;

namespace KashPay.Application.Common.Interfaces.Repositories
{
    public interface IUserRepository
    {
        // === Commands ===
        Task Add(User user);

        // === Queries ===
        Task<User?> GetUserByCredentialsAsync(string credential, CancellationToken ct);
        Task<User?> GetUserByIdAsync(Guid userId, CancellationToken ct);
        Task<bool> UserExistByEmailAsync(string email, CancellationToken ct);
        Task<bool> UserExistByCpfAsync(string cpf, CancellationToken ct);
    }
}