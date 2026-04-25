using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KashPay.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace KashPay.Infrastructure.Data.Configurations
{
    public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
    {
        public void Configure(EntityTypeBuilder<Wallet> builder)
        {
            builder.ToTable("wallets");

            builder.HasKey(x => x.Id);

            builder.Property(ui => ui.UserId)
                .IsRequired();

            builder.Property(b => b.Balance)
                .IsRequired();

            builder.Property(ca => ca.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("now()");

            // Relations
            builder.HasOne(u => u.User)
                .WithOne()
                .HasForeignKey<Wallet>(fk => fk.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}