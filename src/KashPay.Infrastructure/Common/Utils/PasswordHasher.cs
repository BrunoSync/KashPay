using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KashPay.Application.Common.Interfaces.Infrastructure;

namespace KashPay.Infrastructure.Common.Utils
{
    public class PasswordHasher : IPasswordHasher
    {
        public string Hash(string password)
        => BCrypt.Net.BCrypt.HashPassword(password);

        public bool Validate(string password, string hash)
        => BCrypt.Net.BCrypt.Verify(password, hash);
    }
}