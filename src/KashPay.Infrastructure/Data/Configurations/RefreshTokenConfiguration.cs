using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KashPay.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KashPay.Infrastructure.Data.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("refresh_tokens");

            builder.HasKey(x => x.Id);

            builder.Property(ui => ui.UserId)
                .IsRequired();

            builder.Property(t => t.Token)
                .IsRequired();

            builder.Property(ea => ea.ExpiresAt)
                .IsRequired();

            builder.Property(ir => ir.IsRevoked)
                .IsRequired();
        }
    }
}