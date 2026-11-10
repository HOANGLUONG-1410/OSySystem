using BusinessObjects.Data;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace Web_API.DatabaseSeeder
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAdminAsync(
            IServiceProvider services)
        {
            using var scope = services.CreateScope();

            var context = scope.ServiceProvider
                .GetRequiredService<BunBoDbContext>();

            // Lấy danh sách Kitchen để gán KitchenId cho account
            var kitchens = await context.Kitchens
                .Where(k => k.IsActive)
                .ToListAsync();

            var centralKitchen = kitchens
                .FirstOrDefault(k => k.IsCentralKitchen);

            var branchKitchen = kitchens
                .FirstOrDefault(k => !k.IsCentralKitchen);

            // =========================================================
            // 1. SystemAdmin
            // =========================================================

            if (!await context.SystemAccounts
                .AnyAsync(x => x.Role == AccountRole.SystemAdmin))
            {
                var admin = new SystemAccount
                {
                    Username = "admin",
                    PasswordHash =
                        BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    FullName = "System Administrator",
                    Role = AccountRole.SystemAdmin,
                    KitchenId = null,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                await context.SystemAccounts.AddAsync(admin);
            }

            // =========================================================
            // 2. KitchenManager
            // =========================================================

            if (!await context.SystemAccounts
                .AnyAsync(x => x.Username == "kitchen.mgr"))
            {
                var km = new SystemAccount
                {
                    Username = "kitchen.mgr",
                    PasswordHash =
                        BCrypt.Net.BCrypt.HashPassword("Kitchen@123"),
                    FullName = "Quản lý bếp tổng",
                    Role = AccountRole.KitchenManager,
                    KitchenId = centralKitchen?.KitchenId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                await context.SystemAccounts.AddAsync(km);
            }

            // =========================================================
            // 3. BranchManager
            // =========================================================

            if (!await context.SystemAccounts
                .AnyAsync(x => x.Username == "branch.mgr"))
            {
                var bm = new SystemAccount
                {
                    Username = "branch.mgr",
                    PasswordHash =
                        BCrypt.Net.BCrypt.HashPassword("Branch@123"),
                    FullName = "Quản lý chi nhánh",
                    Role = AccountRole.BranchManager,
                    KitchenId = branchKitchen?.KitchenId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                await context.SystemAccounts.AddAsync(bm);
            }

            // =========================================================
            // 4. Staff
            // =========================================================

            if (!await context.SystemAccounts
                .AnyAsync(x => x.Username == "staff"))
            {
                var staff = new SystemAccount
                {
                    Username = "staff",
                    PasswordHash =
                        BCrypt.Net.BCrypt.HashPassword("Staff@123"),
                    FullName = "Nhân viên chi nhánh",
                    Role = AccountRole.Staff,
                    KitchenId = branchKitchen?.KitchenId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                await context.SystemAccounts.AddAsync(staff);
            }

            // =========================================================
            // 5. Sample Schedules (cho staff)
            // =========================================================

            var staffAccount = await context.SystemAccounts
                .FirstOrDefaultAsync(x => x.Username == "staff");

            if (staffAccount != null && !await context.Schedules.AnyAsync())
            {
                var today = DateTime.UtcNow.Date;
                var schedules = new[]
                {
                    new Schedule
                    {
                        KitchenId = staffAccount.KitchenId ?? branchKitchen!.KitchenId,
                        StaffAccountId = staffAccount.AccountId,
                        WorkDate = today.AddDays(1),
                        ShiftType = "Sáng",
                        Note = "Ca sáng - phục vụ bàn",
                        CreatedByAccountId = staffAccount.AccountId,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Schedule
                    {
                        KitchenId = staffAccount.KitchenId ?? branchKitchen!.KitchenId,
                        StaffAccountId = staffAccount.AccountId,
                        WorkDate = today.AddDays(2),
                        ShiftType = "Chiều",
                        Note = "Ca chiều - phụ bếp",
                        CreatedByAccountId = staffAccount.AccountId,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Schedule
                    {
                        KitchenId = staffAccount.KitchenId ?? branchKitchen!.KitchenId,
                        StaffAccountId = staffAccount.AccountId,
                        WorkDate = today.AddDays(3),
                        ShiftType = "Tối",
                        CreatedByAccountId = staffAccount.AccountId,
                        CreatedAt = DateTime.UtcNow
                    }
                };

                await context.Schedules.AddRangeAsync(schedules);
            }

            await context.SaveChangesAsync();
        }
    }
}
