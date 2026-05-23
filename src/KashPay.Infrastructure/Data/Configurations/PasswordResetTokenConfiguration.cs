using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KashPay.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KashPay.Infrastructure.Data.Configurations
{
    public class PasswordResetTokenConfiguration : IEntityTypeConfiguration<PasswordResetToken>
    {
        public void Configure(EntityTypeBuilder<PasswordResetToken> builder)
        {
            builder.ToTable("password_reset_tokens");

            builder.HasKey(x => x.Id);

            builder.Property(t => t.Token)
                .IsRequired();

            builder.Property(u => u.UserId)
                .IsRequired();

            builder.Property(e => e.Expiration)
                .IsRequired();

            builder.Property(ir => ir.IsRevoked)
                .IsRequired();

            // Relations
            builder.HasOne(u => u.User)
                .WithMany()
                .HasForeignKey(ui => ui.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Index
            builder.HasIndex(t => t.Token)
                .IsUnique();
        }
    }
}