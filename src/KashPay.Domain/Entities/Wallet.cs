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
            AccountNumber = GenerateAccountNumber();
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
        public string AccountNumber { get; init; } = string.Empty;
        public decimal Balance { get; private set; }
        public DateTime CreatedAt { get; init; }

        // Methods
        public string GenerateAccountNumber()
        {
            var number = Random.Shared.Next(10000, 9999999);
            var digit = Random.Shared.Next(10, 99);

            return $"{number}-{digit}";
        }

        public void Credit(decimal amount)
        => Balance += amount;

        public void Debit(decimal amount)
        => Balance -= amount;
    }
}