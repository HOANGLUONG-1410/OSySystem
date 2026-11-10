using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;

namespace Repositories.Interfaces
{
    public interface IIngredientRepository
    {
        // Lấy toàn bộ nguyên liệu đang hoạt động
        Task<List<Ingredient>> GetAllAsync();

        // Lấy nguyên liệu theo id
        Task<Ingredient?> GetByIdAsync(int ingredientId);

        // Thêm nguyên liệu mới
        Task AddAsync(Ingredient ingredient);

        // Cập nhật thông tin nguyên liệu
        void Update(Ingredient ingredient);

        // Xóa mềm nguyên liệu
        void SoftDelete(Ingredient ingredient);
    }
}
