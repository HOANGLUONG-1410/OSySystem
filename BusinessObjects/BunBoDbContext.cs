using System.Collections.Generic;
using System.Reflection.Emit;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace BusinessObjects.Data
{
    public class BunBoDbContext : DbContext
    {
        public BunBoDbContext(DbContextOptions<BunBoDbContext> options)
            : base(options)
        {
        }

        public DbSet<Kitchen> Kitchens { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<InventoryLog> InventoryLogs { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Adjustment> Adjustments { get; set; }
        public DbSet<TransferRequest> TransferRequests { get; set; }
        public DbSet<TransferRequestDetail> TransferRequestDetails { get; set; }
        public DbSet<SystemAccount> SystemAccounts { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<ScheduleLog> ScheduleLogs { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Kitchen
            modelBuilder.Entity<Kitchen>()
                .HasKey(x => x.KitchenId);

            // Ingredient
            modelBuilder.Entity<Ingredient>()
                .HasKey(x => x.IngredientId);

            modelBuilder.Entity<Ingredient>()
                .Property(x => x.AverageCost)
                .HasColumnType("decimal(18,2)");

            // MenuItem
            modelBuilder.Entity<MenuItem>()
                .HasKey(x => x.MenuItemId);

            modelBuilder.Entity<MenuItem>()
                .Property(x => x.SellingPrice)
                .HasColumnType("decimal(18,2)");

            // Recipe
            modelBuilder.Entity<Recipe>()
                .HasKey(x => x.RecipeId);

            modelBuilder.Entity<Recipe>()
                .Property(x => x.QuantityRequired)
                .HasColumnType("decimal(18,3)");

            modelBuilder.Entity<Recipe>()
                .HasOne(x => x.MenuItem)
                .WithMany(x => x.Recipes)
                .HasForeignKey(x => x.MenuItemId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Recipe>()
                .HasOne(x => x.Ingredient)
                .WithMany(x => x.Recipes)
                .HasForeignKey(x => x.IngredientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Recipe>()
                .HasIndex(x => new { x.MenuItemId, x.IngredientId })
                .IsUnique();

            // Inventory
            modelBuilder.Entity<Inventory>()
                .HasKey(x => x.InventoryId);

            modelBuilder.Entity<Inventory>()
                .Property(x => x.Quantity)
                .HasColumnType("decimal(18,3)");

            modelBuilder.Entity<Inventory>()
                .HasOne(x => x.Kitchen)
                .WithMany(x => x.Inventories)
                .HasForeignKey(x => x.KitchenId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Inventory>()
                .HasOne(x => x.Ingredient)
                .WithMany(x => x.Inventories)
                .HasForeignKey(x => x.IngredientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Inventory>()
                .HasIndex(x => new { x.KitchenId, x.IngredientId })
                .IsUnique();

            // InventoryLog
            modelBuilder.Entity<InventoryLog>()
                .HasKey(x => x.InventoryLogId);

            modelBuilder.Entity<InventoryLog>()
                .Property(x => x.QuantityChange)
                .HasColumnType("decimal(18,3)");

            modelBuilder.Entity<InventoryLog>()
                .Property(x => x.QuantityBefore)
                .HasColumnType("decimal(18,3)");

            modelBuilder.Entity<InventoryLog>()
                .Property(x => x.QuantityAfter)
                .HasColumnType("decimal(18,3)");

            modelBuilder.Entity<InventoryLog>()
                .HasOne(x => x.Kitchen)
                .WithMany()
                .HasForeignKey(x => x.KitchenId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<InventoryLog>()
                .HasOne(x => x.Ingredient)
                .WithMany()
                .HasForeignKey(x => x.IngredientId)
                .OnDelete(DeleteBehavior.Restrict);

            // Order
            modelBuilder.Entity<Order>()
                .HasKey(x => x.OrderId);

            modelBuilder.Entity<Order>()
                .Property(x => x.TotalAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Order>()
                .HasOne(x => x.Kitchen)
                .WithMany(x => x.Orders)
                .HasForeignKey(x => x.KitchenId)
                .OnDelete(DeleteBehavior.Restrict);

            // OrderDetail
            modelBuilder.Entity<OrderDetail>()
                .HasKey(x => x.OrderDetailId);

            modelBuilder.Entity<OrderDetail>()
                .Property(x => x.UnitPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<OrderDetail>()
                .Property(x => x.SubTotal)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<OrderDetail>()
                .HasOne(x => x.Order)
                .WithMany(x => x.OrderDetails)
                .HasForeignKey(x => x.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(x => x.MenuItem)
                .WithMany(x => x.OrderDetails)
                .HasForeignKey(x => x.MenuItemId)
                .OnDelete(DeleteBehavior.Restrict);

            // Adjustment
            modelBuilder.Entity<Adjustment>()
                .HasKey(x => x.AdjustmentId);

            modelBuilder.Entity<Adjustment>()
                .Property(x => x.SystemQuantity)
                .HasColumnType("decimal(18,3)");

            modelBuilder.Entity<Adjustment>()
                .Property(x => x.ActualQuantity)
                .HasColumnType("decimal(18,3)");

            modelBuilder.Entity<Adjustment>()
                .Property(x => x.DifferenceQuantity)
                .HasColumnType("decimal(18,3)");

            modelBuilder.Entity<Adjustment>()
                .HasOne(x => x.Kitchen)
                .WithMany()
                .HasForeignKey(x => x.KitchenId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Adjustment>()
                .HasOne(x => x.Ingredient)
                .WithMany()
                .HasForeignKey(x => x.IngredientId)
                .OnDelete(DeleteBehavior.Restrict);

            // TransferRequest
            modelBuilder.Entity<TransferRequest>()
                .HasKey(x => x.TransferRequestId);

            modelBuilder.Entity<TransferRequest>()
                .HasOne(x => x.FromKitchen)
                .WithMany()
                .HasForeignKey(x => x.FromKitchenId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TransferRequest>()
                .HasOne(x => x.ToKitchen)
                .WithMany()
                .HasForeignKey(x => x.ToKitchenId)
                .OnDelete(DeleteBehavior.Restrict);

            // TransferRequestDetail
            modelBuilder.Entity<TransferRequestDetail>()
                .HasKey(x => x.TransferRequestDetailId);

            modelBuilder.Entity<TransferRequestDetail>()
                .Property(x => x.Quantity)
                .HasColumnType("decimal(18,3)");

            modelBuilder.Entity<TransferRequestDetail>()
                .HasOne(x => x.TransferRequest)
                .WithMany(x => x.TransferRequestDetails)
                .HasForeignKey(x => x.TransferRequestId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TransferRequestDetail>()
                .HasOne(x => x.Ingredient)
                .WithMany()
                .HasForeignKey(x => x.IngredientId)
                .OnDelete(DeleteBehavior.Restrict);

            // SystemAccount
            modelBuilder.Entity<SystemAccount>()
                .HasKey(x => x.AccountId);

            // Username không được trùng nhau
            // Dùng khi login để tìm đúng một tài khoản
            modelBuilder.Entity<SystemAccount>()
                .HasIndex(x => x.Username)
                .IsUnique();

            modelBuilder.Entity<SystemAccount>()
                .Property(x => x.Username)
                .HasMaxLength(50)
                .IsRequired();

            modelBuilder.Entity<SystemAccount>()
                .Property(x => x.PasswordHash)
                .HasMaxLength(255)
                .IsRequired();

            modelBuilder.Entity<SystemAccount>()
                .Property(x => x.FullName)
                .HasMaxLength(100)
                .IsRequired();

            // Một Kitchen có nhiều Account
            // Account có thể không thuộc Kitchen nào nếu là SystemAdmin
            modelBuilder.Entity<SystemAccount>()
                .HasOne(x => x.Kitchen)
                .WithMany(x => x.SystemAccounts)
                .HasForeignKey(x => x.KitchenId)
                .OnDelete(DeleteBehavior.Restrict);

            // Một Order có thể được tạo bởi một SystemAccount
            modelBuilder.Entity<Order>()
                .HasOne(x => x.CreatedByAccount)
                .WithMany()
                .HasForeignKey(x => x.CreatedByAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            // Một Adjustment được thực hiện bởi một tài khoản
            modelBuilder.Entity<Adjustment>()
                .HasOne(x => x.CreatedByAccount)
                .WithMany()
                .HasForeignKey(x => x.CreatedByAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            // InventoryLog ghi lại tài khoản tạo ra biến động kho
            modelBuilder.Entity<InventoryLog>()
                .HasOne(x => x.PerformedByAccount)
                .WithMany()
                .HasForeignKey(x => x.PerformedByAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            // Người tạo phiếu điều chuyển
            modelBuilder.Entity<TransferRequest>()
                .HasOne(x => x.CreatedByAccount)
                .WithMany()
                .HasForeignKey(x => x.CreatedByAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            // Người xác nhận xuất hàng
            modelBuilder.Entity<TransferRequest>()
                .HasOne(x => x.SentByAccount)
                .WithMany()
                .HasForeignKey(x => x.SentByAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            // Người xác nhận nhận hàng
            modelBuilder.Entity<TransferRequest>()
                .HasOne(x => x.ReceivedByAccount)
                .WithMany()
                .HasForeignKey(x => x.ReceivedByAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            // Schedule
            modelBuilder.Entity<Schedule>()
                .HasKey(x => x.ScheduleId);

            modelBuilder.Entity<Schedule>()
                .HasOne(x => x.Kitchen)
                .WithMany()
                .HasForeignKey(x => x.KitchenId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Schedule>()
                .HasOne(x => x.StaffAccount)
                .WithMany()
                .HasForeignKey(x => x.StaffAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Schedule>()
                .HasOne(x => x.CreatedByAccount)
                .WithMany()
                .HasForeignKey(x => x.CreatedByAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            // ScheduleLog (no FK to Schedule — audit log must persist after deletion)
            modelBuilder.Entity<ScheduleLog>()
                .HasKey(x => x.ScheduleLogId);

            modelBuilder.Entity<ScheduleLog>()
                .HasOne(x => x.PerformedByAccount)
                .WithMany()
                .HasForeignKey(x => x.PerformedByAccountId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
