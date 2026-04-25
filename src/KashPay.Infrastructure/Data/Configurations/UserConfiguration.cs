using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KashPay.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KashPay.Infrastructure.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");

            builder.HasKey(x => x.Id);

            builder.Property(fn => fn.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(ln => ln.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(254);

            builder.Property(hc => hc.HashCpf)
                .IsRequired();

            builder.Property(hp => hp.HashPassword)
                .IsRequired();

            builder.Property(ib => ib.IsBlocked)
                .IsRequired();

            builder.Property(ja => ja.JoinedAt)
                .IsRequired()
                .HasDefaultValueSql("now()");
        }
    }
}