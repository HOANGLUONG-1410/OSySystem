using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.DTOs.Adjustments;

namespace Services.Interfaces
{
    public interface IAdjustmentService
    {
        // Lấy phiếu điều chỉnh theo phạm vi quyền
        Task<List<AdjustmentResponse>>
            GetAccessibleAdjustmentsAsync();

        // Lấy lịch sử điều chỉnh của một Kitchen
        Task<List<AdjustmentResponse>>
            GetByKitchenIdAsync(int kitchenId);

        // Tạo phiếu điều chỉnh và cập nhật kho
        Task<AdjustmentResponse> CreateAsync(
            CreateAdjustmentRequest request);
    }
}
