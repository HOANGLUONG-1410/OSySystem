using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;

namespace Repositories.Interfaces
{
    public interface IAdjustmentRepository
    {
        // Lấy toàn bộ lịch sử điều chỉnh kho
        // Dùng cho SystemAdmin xem audit toàn hệ thống
        Task<List<Adjustment>> GetAllAsync();

        // Lấy các phiếu điều chỉnh thuộc một Kitchen
        // Dùng cho BranchManager xem lịch sử điều chỉnh của quán mình
        Task<List<Adjustment>> GetByKitchenIdAsync(int kitchenId);

        // Thêm một phiếu điều chỉnh kho mới
        // Service sẽ tính toán số lượng chênh lệch trước khi gọi Repository
        Task AddAsync(Adjustment adjustment);
    }
}
