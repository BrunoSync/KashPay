using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KashPay.Domain.Entities;

namespace KashPay.Application.Common.Interfaces.Infrastructure.Repositories
{
    public interface ITransactionRepository
    {
        // === Commands ===
        Task Add(Transaction transaction);
    }
}