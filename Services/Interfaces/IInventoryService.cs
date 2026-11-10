using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.DTOs.Inventories;

namespace Services.Interfaces
{
    public interface IInventoryService
    {
        // Lấy tồn kho theo phạm vi quyền của người đang đăng nhập
        Task<List<InventoryResponse>> GetAccessibleInventoriesAsync();

        // Lấy tồn kho của một Kitchen cụ thể
        // Service sẽ kiểm tra người dùng có được xem Kitchen đó không
        Task<List<InventoryResponse>> GetByKitchenIdAsync(int kitchenId);

        // Lấy tồn kho của một nguyên liệu tại một Kitchen
        Task<InventoryResponse> GetByKitchenAndIngredientAsync(
            int kitchenId,
            int ingredientId);

        // Khởi tạo dòng tồn kho cho nguyên liệu tại một Kitchen
        Task<InventoryResponse> CreateInventoryAsync(
            CreateInventoryRequest request);

        // Nhập thêm nguyên liệu và tính lại giá vốn bình quân
        Task<InventoryResponse> ImportInventoryAsync(
            ImportInventoryRequest request);

        // Lấy lịch sử biến động kho theo phạm vi quyền
        Task<List<InventoryLogResponse>> GetAccessibleLogsAsync();

        // Lấy lịch sử kho của một Kitchen cụ thể
        Task<List<InventoryLogResponse>> GetLogsByKitchenIdAsync(
            int kitchenId);
    }
}
