using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KashPay.Application.Common.Interfaces.Infrastructure.Repositories
{
    public interface IUnitOfWork
    {
        Task CommitAsync(CancellationToken ct);
        Task BeginTransactionAsync(CancellationToken ct);
    }
}