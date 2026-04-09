using Microsoft.EntityFrameworkCore;
using AdminService.Domain.Entities;

namespace AdminService.Infrastructure.Persistence
{
    public class AdminDbContext : DbContext
    {
        public AdminDbContext(DbContextOptions<AdminDbContext> options) : base(options)
        {
        }

        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<OrderSummary> OrderSummaries { get; set; }
        public DbSet<UserSummary> UserSummaries { get; set; }
        public DbSet<UserRoleApprovalRequest> UserRoleApprovalRequests { get; set; }
        public DbSet<RestaurantSummary> RestaurantSummaries { get; set; }
        public DbSet<RestaurantApprovalRequest> RestaurantApprovalRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.ToTable("AuditLogs");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.OrderId).IsRequired();
                entity.Property(e => e.PerformedByAdminId).HasMaxLength(255).IsRequired();
                entity.Property(e => e.Action).HasMaxLength(500).IsRequired();
                entity.Property(e => e.Reason).HasMaxLength(1000);
                entity.Property(e => e.PerformedAt).IsRequired();

                entity.HasIndex(e => e.OrderId);
                entity.HasIndex(e => e.PerformedAt);
            });

            modelBuilder.Entity<OrderSummary>(entity =>
            {
                entity.ToTable("OrderSummaries");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.OrderId).IsRequired();
                entity.Property(e => e.CustomerId).HasMaxLength(255).IsRequired();
                entity.Property(e => e.RestaurantName).HasMaxLength(255).IsRequired();
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(30).IsRequired();
                entity.Property(e => e.PlacedAt).IsRequired();
                entity.Property(e => e.LastUpdatedAt).IsRequired();

                entity.HasIndex(e => e.OrderId).IsUnique();
                entity.HasIndex(e => e.CustomerId);
                entity.HasIndex(e => e.PlacedAt);
            });

            modelBuilder.Entity<UserSummary>(entity =>
            {
                entity.ToTable("UserSummaries");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.UserId).HasMaxLength(450).IsRequired();
                entity.Property(e => e.FullName).HasMaxLength(255).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
                entity.Property(e => e.PhoneNo).HasMaxLength(20).IsRequired();
                entity.Property(e => e.Role).HasMaxLength(50).IsRequired();

                entity.HasIndex(e => e.UserId).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.Role);
            });

            modelBuilder.Entity<UserRoleApprovalRequest>(entity =>
            {
                entity.ToTable("UserRoleApprovalRequests");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
                entity.Property(e => e.FullName).HasMaxLength(255).IsRequired();
                entity.Property(e => e.Role).HasMaxLength(50).IsRequired();
                entity.Property(e => e.ApprovedByAdminId).HasMaxLength(450);
                entity.Property(e => e.RejectionReason).HasMaxLength(500);

                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.IsApproved);
            });

            modelBuilder.Entity<RestaurantSummary>(entity =>
            {
                entity.ToTable("RestaurantSummaries");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.RestaurantId).IsRequired();
                entity.Property(e => e.OwnerId).HasMaxLength(450).IsRequired();
                entity.Property(e => e.Name).HasMaxLength(255).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
                entity.Property(e => e.PhoneNumber).HasMaxLength(20).IsRequired();

                entity.HasIndex(e => e.RestaurantId).IsUnique();
                entity.HasIndex(e => e.OwnerId);
                entity.HasIndex(e => e.IsActive);
            });

            modelBuilder.Entity<RestaurantApprovalRequest>(entity =>
            {
                entity.ToTable("RestaurantApprovalRequests");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.RestaurantId).IsRequired();
                entity.Property(e => e.OwnerId).HasMaxLength(450).IsRequired();
                entity.Property(e => e.RestaurantName).HasMaxLength(255).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
                entity.Property(e => e.PhoneNumber).HasMaxLength(20).IsRequired();
                entity.Property(e => e.ApprovedByAdminId).HasMaxLength(450);
                entity.Property(e => e.RejectionReason).HasMaxLength(500);

                entity.HasIndex(e => e.RestaurantId).IsUnique();
                entity.HasIndex(e => e.IsApproved);
            });
        }
    }
}
