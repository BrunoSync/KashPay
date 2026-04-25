using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KashPay.Domain.Enums;

namespace KashPay.Domain.Entities
{
    public class Transaction
    {
        // Constructor
        public Transaction(Guid? fromAccountId, Guid? toAccountId, decimal amount, TransactionType type)
        {
            Id = Guid.NewGuid();
            FromAccountId = fromAccountId;
            ToAccountId = toAccountId;
            Amount = amount;
            Type = type;
            CreatedAt = DateTime.UtcNow;
        }

        // EF Constructor
        public Transaction()
        {
            
        }
        
        // Properties
        public Guid Id { get; init; }
        public Guid? FromAccountId { get; init; }
        public Guid? ToAccountId { get; init; }
        public decimal Amount { get; init; }
        public TransactionType Type { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}