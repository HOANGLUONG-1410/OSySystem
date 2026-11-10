using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;

namespace Repositories.Interfaces
{
    public interface IKitchenRepository
    {
        // Lấy toàn bộ bếp và chi nhánh đang hoạt động
        // Dùng cho Dashboard hoặc danh sách chọn Kitchen
        Task<List<Kitchen>> GetAllAsync();

        // Lấy Kitchen theo id
        // Dùng khi tạo tài khoản, tạo đơn hàng hoặc điều chuyển kho
        Task<Kitchen?> GetByIdAsync(int kitchenId);

        // Thêm bếp tổng hoặc chi nhánh mới
        Task AddAsync(Kitchen kitchen);

        // Cập nhật thông tin Kitchen
        void Update(Kitchen kitchen);

        // Xóa mềm Kitchen
        void SoftDelete(Kitchen kitchen);
    }
}
