using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Data;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects.DAOs
{
    public class InventoryLogDAO
    {
        private readonly BunBoDbContext _context;

        public InventoryLogDAO(BunBoDbContext context)
        {
            _context = context;
        }

        // Lấy lịch sử biến động kho toàn hệ thống
        // Dùng cho màn hình Audit / Inventory History
        public async Task<List<InventoryLog>> GetAllAsync()
        {
            return await _context.InventoryLogs
                .AsNoTracking()
                .Include(x => x.Kitchen)
                .Include(x => x.Ingredient)
                .Include(x => x.PerformedByAccount)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Lấy lịch sử kho theo chi nhánh
        // Dùng khi quản lý muốn xem biến động kho của Quán A hoặc Bếp tổng
        public async Task<List<InventoryLog>> GetByKitchenIdAsync(int kitchenId)
        {
            return await _context.InventoryLogs
                .AsNoTracking()
                .Include(x => x.Ingredient)
                .Include(x => x.PerformedByAccount)
                .Where(x => x.KitchenId == kitchenId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Lấy lịch sử kho của một nguyên liệu
        // Dùng khi muốn truy vết vì sao tồn kho thịt bò bị âm hoặc sai lệch
        public async Task<List<InventoryLog>> GetByIngredientIdAsync(int ingredientId)
        {
            return await _context.InventoryLogs
                .AsNoTracking()
                .Include(x => x.Kitchen)
                .Include(x => x.PerformedByAccount)
                .Where(x => x.IngredientId == ingredientId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Thêm log biến động kho
        // Mọi thao tác nhập, bán, điều chỉnh, điều chuyển đều phải gọi hàm này
        public async Task AddAsync(InventoryLog log)
        {
            await _context.InventoryLogs.AddAsync(log);
        }
    }
}
