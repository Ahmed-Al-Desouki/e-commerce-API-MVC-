using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Cart> Carts => Set<Cart>();
        public DbSet<CartItem> CartItems => Set<CartItem>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<Payment> Payments => Set<Payment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasOne(u => u.Cart)
                .WithOne(c => c.User)
                .HasForeignKey<Cart>(c => c.UserId);

            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Order>()
                .Property(o => o.Status)
                .HasConversion<int>()
                .HasDefaultValue(OrderStatus.PendingPayment);

            modelBuilder.Entity<Payment>()
                .Property(p => p.Status)
                .HasConversion<int>()
                .HasDefaultValue(PaymentStatus.Pending);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Payment)
                .WithOne(p => p.Order)
                .HasForeignKey<Payment>(p => p.OrderId);

            modelBuilder.Entity<Payment>()
                .HasIndex(p => p.MerchantOrderId)
                .IsUnique();
        }
    }
}
