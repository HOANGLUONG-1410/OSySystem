using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;

namespace Repositories.Interfaces
{
    public interface IOrderDetailRepository
    {
        // Lấy danh sách món trong một đơn hàng
        Task<List<OrderDetail>> GetByOrderIdAsync(int orderId);

        // Thêm nhiều món vào đơn
        Task AddRangeAsync(List<OrderDetail> orderDetails);

        // Xóa một dòng chi tiết
        // Chỉ dùng khi Order còn Pending
        void Delete(OrderDetail orderDetail);
    }
}
