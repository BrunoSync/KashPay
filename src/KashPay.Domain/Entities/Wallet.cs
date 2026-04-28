using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KashPay.Domain.Entities
{
    public class Wallet
    {
        // Constructor
        public Wallet(Guid userId)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Balance = 0;
            CreatedAt = DateTime.UtcNow;
        }

        // EF Constructor
        public Wallet()
        {
            
        }

        // Properties
        public Guid Id { get; init; }
        public Guid UserId { get; init; }
        public User User { get; init; } = null!;
        public decimal Balance { get; private set; }
        public DateTime CreatedAt { get; init; }

        // Methods
        public void Credit(decimal amount)
        => Balance += amount;

        public void Debit(decimal amount)
        => Balance -= amount;
    }
}