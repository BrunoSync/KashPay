using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KashPay.Application.Common.Interfaces.Jwt;
using KashPay.Infrastructure.Common.Services;
using KashPay.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace KashPay.Infrastructure.Extension
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Jwt
            services.AddScoped<IJwtService, JwtService>();

                // Read JWT configs
            var jwtKey    = configuration["JWT:KEY"]!;
            var jwtIssuer  = configuration["JWT:ISSUER"]!;
            var jwtAud     = configuration["JWT:AUDIENCE"]!;
                // Configures JWT authentication middleware
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer           = true,
                    ValidateAudience         = true,
                    ValidateLifetime         = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer              = jwtIssuer,
                    ValidAudience            = jwtAud,
                    IssuerSigningKey         = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey)
                    )
                };
            });

            services.AddAuthorization();

            // Database
            var connectionString = Environment.GetEnvironmentVariable("CONNECTIONSTRINGS__DEFAULTCONNECTION");
            services.AddDbContext<AppDbContext>(Options =>
                Options.UseNpgsql(connectionString)
            );

            // Return
            return services;
        } 
    }
}