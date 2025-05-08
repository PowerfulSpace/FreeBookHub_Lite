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

            builder.HasKey(i => new { i.BookId, i.CartUserId }); // Композитный ключ

            builder.Property(ci => ci.BookId)
                   .IsRequired();

            builder.Property(ci => ci.Quantity)
                   .IsRequired();
        }
    }
}
