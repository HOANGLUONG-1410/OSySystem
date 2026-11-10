using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;

namespace Repositories.Interfaces
{
    public interface IInventoryLogRepository
    {
        // Lấy toàn bộ lịch sử kho
        Task<List<InventoryLog>> GetAllAsync();

        // Lấy lịch sử kho theo chi nhánh
        Task<List<InventoryLog>> GetByKitchenIdAsync(int kitchenId);

        // Lấy lịch sử biến động của một nguyên liệu
        Task<List<InventoryLog>> GetByIngredientIdAsync(int ingredientId);

        // Ghi thêm một bản ghi lịch sử kho
        Task AddAsync(InventoryLog inventoryLog);
    }
}
