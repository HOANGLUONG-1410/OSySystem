using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;

namespace Repositories.Interfaces
{
    public interface IMenuItemRepository
    {
        // Lấy danh sách món ăn đang bán
        Task<List<MenuItem>> GetAllAsync();

        // Lấy món ăn theo id
        Task<MenuItem?> GetByIdAsync(int menuItemId);

        // Lấy món ăn kèm toàn bộ công thức nguyên liệu
        // Dùng khi confirm đơn hàng
        Task<MenuItem?> GetWithRecipeAsync(int menuItemId);

        // Thêm món ăn mới
        Task AddAsync(MenuItem menuItem);

        // Cập nhật món ăn
        void Update(MenuItem menuItem);

        // Xóa mềm món ăn
        void SoftDelete(MenuItem menuItem);
    }
}
