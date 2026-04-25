using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KashPay.Domain.Entities
{
    public class RefreshToken
    {
        // Constructor
        public RefreshToken(Guid userId, string token, DateTime expiresAt)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Token = token;
            ExpiresAt = expiresAt;
            IsRevoked = false;
        }

        // EF Constructor
        public RefreshToken()
        {
            
        }

        // Properties
        public Guid Id { get; init; }
        public Guid UserId { get; init; }
        public string Token { get; init; } = string.Empty;
        public DateTime ExpiresAt { get; init; }
        public bool IsRevoked { get; private set; }

        // Methods
        public void Revoke()
        => IsRevoked = true;
    }
}