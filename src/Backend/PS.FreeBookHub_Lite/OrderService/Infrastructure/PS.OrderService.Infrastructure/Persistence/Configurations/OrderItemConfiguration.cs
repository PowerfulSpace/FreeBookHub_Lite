using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PS.OrderService.Domain.Entities;

namespace PS.OrderService.Infrastructure.Persistence.Configurations
{
    public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("OrderItems");

            builder.HasKey(oi => new { oi.OrderId, oi.BookId });

            builder.Property(oi => oi.UnitPrice)
                   .IsRequired()
                   .HasPrecision(18, 2);

            builder.Property(oi => oi.Quantity)
                   .IsRequired();

            builder.HasOne(oi => oi.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(o => o.OrderId);

            builder.HasIndex(oi => oi.BookId);
        }
    }
}
