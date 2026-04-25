using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KashPay.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KashPay.Infrastructure.Data.Configurations
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.ToTable("transactions");

            builder.HasKey(x => x.Id);

            builder.Property(a => a.Amount)
                .IsRequired();

            builder.Property(t => t.Type)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(ca => ca.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("now()");
        }
    }
}