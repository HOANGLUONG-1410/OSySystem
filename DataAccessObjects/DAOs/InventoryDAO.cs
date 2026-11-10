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
    public class InventoryDAO
    {
        private readonly BunBoDbContext _context;

        public InventoryDAO(BunBoDbContext context)
        {
            _context = context;
        }

        // Lấy toàn bộ tồn kho
        // Dùng cho Dashboard tổng quan kho
        public async Task<List<Inventory>> GetAllAsync()
        {
            return await _context.Inventories
                .Include(x => x.Kitchen)
                .Include(x => x.Ingredient)
                .ToListAsync();
        }

        // Lấy tồn kho của một chi nhánh/bếp
        // Dùng khi xem kho riêng của Quán A, Quán B, Bếp tổng
        public async Task<List<Inventory>> GetByKitchenIdAsync(int kitchenId)
        {
            return await _context.Inventories
                .Include(x => x.Ingredient)
                .Where(x => x.KitchenId == kitchenId)
                .ToListAsync();
        }

        // Lấy tồn kho cụ thể của 1 nguyên liệu tại 1 chi nhánh
        // Dùng trong ConfirmOrder, Adjustment, Transfer
        public async Task<Inventory?> GetByKitchenAndIngredientAsync(int kitchenId, int ingredientId)
        {
            return await _context.Inventories
                .FirstOrDefaultAsync(x =>
                    x.KitchenId == kitchenId &&
                    x.IngredientId == ingredientId);
        }

        // Thêm dòng tồn kho mới
        // Dùng khi nguyên liệu lần đầu xuất hiện tại một chi nhánh
        public async Task AddAsync(Inventory inventory)
        {
            await _context.Inventories.AddAsync(inventory);
        }

        // Cập nhật số lượng tồn kho
        // Service sẽ tính toán số lượng, DAO chỉ update
        public void Update(Inventory inventory)
        {
            inventory.LastUpdated = DateTime.Now;
            _context.Inventories.Update(inventory);
        }
    }
}
