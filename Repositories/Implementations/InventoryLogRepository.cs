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
    public class InventoryLogRepository : IInventoryLogRepository
    {
        private readonly InventoryLogDAO _inventoryLogDAO;

        public InventoryLogRepository(
            InventoryLogDAO inventoryLogDAO)
        {
            _inventoryLogDAO = inventoryLogDAO;
        }

        // Lấy toàn bộ lịch sử biến động kho
        // Dùng cho SystemAdmin kiểm tra audit
        public Task<List<InventoryLog>> GetAllAsync()
        {
            return _inventoryLogDAO.GetAllAsync();
        }

        // Lấy lịch sử kho của một Kitchen
        public Task<List<InventoryLog>> GetByKitchenIdAsync(int kitchenId)
        {
            return _inventoryLogDAO.GetByKitchenIdAsync(kitchenId);
        }

        // Truy vết biến động của một Ingredient cụ thể
        public Task<List<InventoryLog>> GetByIngredientIdAsync(
            int ingredientId)
        {
            return _inventoryLogDAO.GetByIngredientIdAsync(ingredientId);
        }

        // Ghi log mỗi lần số lượng Inventory thay đổi
        public Task AddAsync(InventoryLog inventoryLog)
        {
            return _inventoryLogDAO.AddAsync(inventoryLog);
        }
    }
}
