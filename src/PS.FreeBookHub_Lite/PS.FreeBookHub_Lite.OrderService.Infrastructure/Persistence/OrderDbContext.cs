﻿using Microsoft.EntityFrameworkCore;
using PS.FreeBookHub_Lite.OrderService.Domain.Entities;

namespace PS.FreeBookHub_Lite.OrderService.Infrastructure.Persistence
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options)
            : base(options)
        {
        }

        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .ApplyConfigurationsFromAssembly(typeof(OrderDbContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
