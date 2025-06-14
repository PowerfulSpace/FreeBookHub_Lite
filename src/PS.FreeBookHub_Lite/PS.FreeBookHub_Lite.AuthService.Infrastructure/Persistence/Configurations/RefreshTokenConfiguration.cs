﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PS.FreeBookHub_Lite.AuthService.Domain.Entities;

namespace PS.FreeBookHub_Lite.AuthService.Infrastructure.Persistence.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Token)
                .IsRequired()
                .HasMaxLength(512)
                .IsUnicode(false);

            builder.Property(t => t.UserId)
                .IsRequired();

            builder.Property(t => t.ExpiresAt)
                .IsRequired();

            builder.Property(t => t.CreatedAt)
                .IsRequired();


            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.HasIndex(t => t.Token)
               .IsUnique();
            builder.HasIndex(t => t.UserId);
            builder.HasIndex(t => t.ExpiresAt);
            builder.HasIndex(rt => new { rt.UserId, rt.IsRevoked, rt.ExpiresAt });
        }
    }
}
