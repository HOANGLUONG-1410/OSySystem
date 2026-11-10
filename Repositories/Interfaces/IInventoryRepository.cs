using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;

namespace Repositories.Interfaces
{
    public interface IInventoryRepository
    {
        // Lấy toàn bộ kho của tất cả chi nhánh
        Task<List<Inventory>> GetAllAsync();

        // Lấy kho của một Kitchen cụ thể
        Task<List<Inventory>> GetByKitchenIdAsync(int kitchenId);

        // Lấy số tồn của một nguyên liệu tại một Kitchen
        Task<Inventory?> GetByKitchenAndIngredientAsync(
            int kitchenId,
            int ingredientId);

        // Tạo dòng tồn kho mới
        Task AddAsync(Inventory inventory);

        // Cập nhật số lượng tồn kho
        void Update(Inventory inventory);
    }
}
