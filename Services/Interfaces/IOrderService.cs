using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Enums;
using Services.DTOs.Orders;

namespace Services.Interfaces
{
    public interface IOrderService
    {
        // Lấy đơn hàng theo phạm vi quyền người dùng
        Task<List<OrderResponse>> GetAccessibleOrdersAsync();

        // Lấy đơn theo trạng thái trong phạm vi Kitchen người dùng
        Task<List<OrderResponse>> GetByStatusAsync(OrderStatus status);

        // Lấy chi tiết một đơn và kiểm tra quyền truy cập Kitchen
        Task<OrderResponse> GetByIdAsync(int orderId);

        // Tạo đơn Pending, chưa trừ kho
        Task<OrderResponse> CreateAsync(CreateOrderRequest request);

        // Xác nhận đơn và trừ kho theo Recipe
        Task<ConfirmOrderResponse> ConfirmAsync(int orderId);

        // Chuyển Confirmed sang Completed
        Task<OrderResponse> CompleteAsync(int orderId);

        // Hủy đơn khi đơn còn Pending
        Task<OrderResponse> CancelAsync(int orderId);
    }
}
