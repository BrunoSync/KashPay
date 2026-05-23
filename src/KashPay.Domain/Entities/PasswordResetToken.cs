using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KashPay.Domain.Entities
{
    public class PasswordResetToken
    {
        // Constructor
        public PasswordResetToken(Guid userId, string token, DateTime expiration)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Token = token;
            Expiration = expiration;
            IsRevoked = false;
        }

        // EF Constructor
        public PasswordResetToken()
        {
            
        }

        // Properties
        public Guid Id { get; init; }
        public Guid UserId { get; init; }
        public User User { get; init; } = null!;
        public string Token { get; init; } = string.Empty;
        public DateTime Expiration { get; init; }
        public bool IsRevoked { get; private set; }

        // Metods
        public void Revoke()
        {
            IsRevoked = true;
        }
    }
}