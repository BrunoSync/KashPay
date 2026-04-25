using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KashPay.Domain.Entities
{
    public class User
    {
        // Constructor
        public User(string firstName, string lastName, string email, string hashCpf, string hashPassword)
        {
            Id = Guid.NewGuid();
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            HashCpf = hashCpf;
            HashPassword = hashPassword;
            IsBlocked = false;
            BlockedAt = null;
            JoinedAt = DateTime.UtcNow;
        }

        // EF Constructor
        public User()
        {
            
        }

        // Properties
        public Guid Id { get; init; }
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string HashCpf { get; init; } = string.Empty;
        public string HashPassword { get; private set; } = string.Empty;
        public bool IsBlocked { get; private set; }
        public DateTime? BlockedAt { get; private set; }
        public DateTime JoinedAt { get; init; }

        // Methods
        public void ChangeHashPassword(string newHashPassword)
        => HashPassword = newHashPassword;

        public void SetBlocked()
        {
            IsBlocked = true;
            BlockedAt = DateTime.UtcNow;
        }

        public void SetUnBlocked()
        {
            IsBlocked = false;
            BlockedAt = null;
        }
    }
}