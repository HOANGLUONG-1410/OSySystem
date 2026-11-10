using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Enums;
using BusinessObjects.Models;

namespace Repositories.Interfaces
{
    public interface IOrderRepository
    {
        // Lấy toàn bộ đơn hàng kèm chi tiết
        // Dùng cho SystemAdmin xem đơn toàn hệ thống
        Task<List<Order>> GetAllAsync();

        // Lấy một đơn hàng kèm OrderDetail và MenuItem
        // Dùng cho màn hình chi tiết và nghiệp vụ ConfirmOrder
        Task<Order?> GetByIdAsync(int orderId);

        // Lấy toàn bộ đơn Pending
        // Dùng cho màn hình bếp xem đơn cần xử lý
        Task<List<Order>> GetPendingOrdersAsync();

        // Lấy đơn hàng theo Kitchen
        // Dùng cho chi nhánh chỉ xem đơn của mình
        Task<List<Order>> GetByKitchenIdAsync(int kitchenId);

        // Lấy đơn theo Kitchen và trạng thái
        // Ví dụ lấy các đơn Pending của Quán A
        Task<List<Order>> GetByKitchenAndStatusAsync(
            int kitchenId,
            OrderStatus status);

        // Thêm Order cùng OrderDetail
        Task AddAsync(Order order);

        // Đánh dấu Order đã thay đổi
        void Update(Order order);
    }
}
