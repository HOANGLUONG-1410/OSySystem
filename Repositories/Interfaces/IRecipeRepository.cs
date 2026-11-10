using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;

namespace Repositories.Interfaces
{
    public interface IRecipeRepository
    {
        // Lấy toàn bộ công thức trong hệ thống
        Task<List<Recipe>> GetAllAsync();

        // Lấy một dòng công thức theo id
        Task<Recipe?> GetByIdAsync(int recipeId);

        // Lấy công thức của một món ăn
        // Dùng để tính nguyên liệu khi xác nhận đơn
        Task<List<Recipe>> GetByMenuItemIdAsync(int menuItemId);

        // Thêm nguyên liệu vào công thức món ăn
        Task AddAsync(Recipe recipe);

        // Cập nhật định lượng nguyên liệu
        void Update(Recipe recipe);

        // Xóa một nguyên liệu khỏi công thức
        void Delete(Recipe recipe);
    }
}
