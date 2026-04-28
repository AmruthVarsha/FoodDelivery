using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Entities;

namespace OrderService.Infrastructure.Persistence;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
    {
    }

    public DbSet<Order> Orders { get; set; }
    public DbSet<RestaurantOrder> RestaurantOrders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<DeliveryAssignment> DeliveryAssignments { get; set; }
    public DbSet<DeliveryAgentProfile> DeliveryAgentProfiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ──────────────────────────────────────────
        // Cart & CartItem
        // ──────────────────────────────────────────
        modelBuilder.Entity<Cart>(entity =>
        {
            entity.ToTable("Carts");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CustomerId).HasMaxLength(450);
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);

            entity.HasMany(c => c.CartItems)
                .WithOne(ci => ci.Cart)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.ToTable("CartItems");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.MenuItemName).HasMaxLength(100);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");
        });

        // ──────────────────────────────────────────
        // Order (Parent) → RestaurantOrders → OrderItems
        // ──────────────────────────────────────────
        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("Orders");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.CustomerId).HasMaxLength(450);
            entity.Property(e => e.CustomerName).HasMaxLength(200);
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(30);
            entity.Property(e => e.Street).HasMaxLength(200);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.State).HasMaxLength(100);
            entity.Property(e => e.Pincode).HasMaxLength(20);
            entity.Property(e => e.DeliveryInstructions).HasMaxLength(300);
            entity.Property(e => e.ScheduledSlot).HasMaxLength(100);
            entity.Property(e => e.CancellationReason).HasMaxLength(500);
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");

            entity.HasMany(o => o.RestaurantOrders)
                .WithOne(ro => ro.Order)
                .HasForeignKey(ro => ro.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(o => o.Payment)
                .WithOne(p => p.Order)
                .HasForeignKey<Payment>(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(o => o.DeliveryAssignment)
                .WithOne(d => d.Order)
                .HasForeignKey<DeliveryAssignment>(d => d.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<RestaurantOrder>(entity =>
        {
            entity.ToTable("RestaurantOrders");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RestaurantName).HasMaxLength(200);
            entity.Property(e => e.RestaurantAddress).HasMaxLength(500);
            entity.Property(e => e.SubTotal).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(30);
            entity.Property(e => e.CancellationReason).HasMaxLength(500);

            entity.HasMany(ro => ro.OrderItems)
                .WithOne(oi => oi.RestaurantOrder)
                .HasForeignKey(oi => oi.RestaurantOrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.ToTable("OrderItems");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.MenuItemName).HasMaxLength(100);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(18,2)");
        });

        // ──────────────────────────────────────────
        // Payment
        // ──────────────────────────────────────────
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.ToTable("Payments");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Method).HasConversion<string>().HasMaxLength(20);
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);
            entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.TransactionReference).HasMaxLength(100);
        });

        // ──────────────────────────────────────────
        // DeliveryAssignment
        // ──────────────────────────────────────────
        modelBuilder.Entity<DeliveryAssignment>(entity =>
        {
            entity.ToTable("DeliveryAssignments");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DeliveryAgentId).HasMaxLength(450);
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);
        });

        // ──────────────────────────────────────────
        // DeliveryAgentProfile
        // ──────────────────────────────────────────
        modelBuilder.Entity<DeliveryAgentProfile>(entity =>
        {
            entity.ToTable("DeliveryAgentProfiles");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AgentUserId).HasMaxLength(450);
            entity.Property(e => e.CurrentPincode).HasMaxLength(20);
            entity.HasIndex(e => e.AgentUserId).IsUnique();
        });
    }
}