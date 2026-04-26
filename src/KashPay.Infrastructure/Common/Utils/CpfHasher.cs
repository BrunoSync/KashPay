using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using KashPay.Application.Common.Interfaces.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace KashPay.Infrastructure.Common.Utils
{
    internal class CpfHasher : ICpfHasher
    {
        private readonly string _secretKey;

        public CpfHasher(IConfiguration config)
        {
            _secretKey = config["CPF:SECRETKEY"]!;
        }

        public string Hash(string cpf) 
        {
            var cleaned  = new string(cpf.Where(char.IsDigit).ToArray());
            var keyBytes = Encoding.UTF8.GetBytes(_secretKey);
            var cpfBytes = Encoding.UTF8.GetBytes(cleaned);

            using var hmac = new HMACSHA256(keyBytes);
            return Convert.ToBase64String(hmac.ComputeHash(cpfBytes));
        }
    }
}