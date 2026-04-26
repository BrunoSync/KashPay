using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using KashPay.Application.Common.Interfaces.Jwt;
using KashPay.Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace KashPay.Infrastructure.Common.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _config;

        public JwtService(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateAccessToken(User user)
        {
            // Take the secret key from the .env file and convert it to bytes.
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["JWT:KEY"]!)
            );

            // Define the signature algorithm: HMAC-SHA256
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Claims (user info)
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            // Mount the token with all the data
            var token = new JwtSecurityToken(
                issuer: _config["JWT:ISSUER"],
                audience: _config["JWT:AUDIENCE"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(_config["JWT:EXPIRATIONMINUTES"]!)),
                signingCredentials: creds
            );

            // Serializes token to string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public (string, int) GenerateRefreshToken()
        {
            // Generate 64 random bytes and convert to Base64
            var bytes = RandomNumberGenerator.GetBytes(64);
            var expiration = int.Parse(_config["JWT:REFRESHTOKENEXPIRATIONDAYS"]!);
            return (Convert.ToBase64String(bytes), expiration);
        }
    }
}