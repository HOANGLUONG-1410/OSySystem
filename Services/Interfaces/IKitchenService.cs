using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.DTOs.Kitchens;

namespace Services.Interfaces
{
    public interface IKitchenService
    {
        // Lấy danh sách bếp và chi nhánh đang hoạt động
        Task<List<KitchenResponse>> GetAllAsync();

        // Lấy chi tiết một Kitchen
        Task<KitchenResponse> GetByIdAsync(int kitchenId);

        // SystemAdmin tạo bếp tổng hoặc chi nhánh
        Task<KitchenResponse> CreateAsync(
            CreateKitchenRequest request);

        // SystemAdmin sửa tên hoặc địa chỉ Kitchen
        Task<KitchenResponse> UpdateAsync(
            int kitchenId,
            UpdateKitchenRequest request);

        // Xóa mềm Kitchen
        Task DeactivateAsync(int kitchenId);
    }
}
