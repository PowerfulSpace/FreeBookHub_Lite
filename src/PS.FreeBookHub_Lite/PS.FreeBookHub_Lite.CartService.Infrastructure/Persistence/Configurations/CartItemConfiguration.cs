using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PS.FreeBookHub_Lite.CartService.Domain.Entities;

namespace PS.FreeBookHub_Lite.CartService.Infrastructure.Persistence.Configurations
{
    public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            builder.ToTable("CartItems");

            builder.HasKey(i => new { i.CartId, i.BookId }); // Композитный ключ

            builder.Property(ci => ci.BookId)
                   .IsRequired();

            builder.Property(ci => ci.Quantity)
                   .IsRequired();

            builder.Property(i => i.UnitPrice)
                  .IsRequired()
                  .HasColumnType("decimal(18,2)");

            builder.HasOne(i => i.Cart)
                .WithMany(i => i.Items)
                .HasForeignKey(i => i.CartId);

            builder.Ignore(i => i.TotalPrice); // Вычисляемое, не сохраняемое
        }
    }
}
