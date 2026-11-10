using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;
using DataAccessObjects.DAOs;
using Repositories.Interfaces;

namespace Repositories.Implementations
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly InventoryDAO _inventoryDAO;

        public InventoryRepository(InventoryDAO inventoryDAO)
        {
            _inventoryDAO = inventoryDAO;
        }

        // Lấy toàn bộ dữ liệu kho cho Dashboard Admin
        public Task<List<Inventory>> GetAllAsync()
        {
            return _inventoryDAO.GetAllAsync();
        }

        // Lấy tồn kho của một chi nhánh
        // Dùng cho BranchManager hoặc Staff xem kho quán mình
        public Task<List<Inventory>> GetByKitchenIdAsync(int kitchenId)
        {
            return _inventoryDAO.GetByKitchenIdAsync(kitchenId);
        }

        // Lấy đúng một nguyên liệu tại đúng một Kitchen
        // Đây là method quan trọng nhất khi cộng/trừ kho
        public Task<Inventory?> GetByKitchenAndIngredientAsync(
            int kitchenId,
            int ingredientId)
        {
            return _inventoryDAO.GetByKitchenAndIngredientAsync(
                kitchenId,
                ingredientId);
        }

        // Thêm Inventory khi một nguyên liệu xuất hiện lần đầu tại chi nhánh
        public Task AddAsync(Inventory inventory)
        {
            return _inventoryDAO.AddAsync(inventory);
        }

        // Cập nhật Quantity và LastUpdated
        public void Update(Inventory inventory)
        {
            _inventoryDAO.Update(inventory);
        }
    }
}
