using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Data;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects.DAOs
{
    public class OrderDetailDAO
    {
        private readonly BunBoDbContext _context;

        public OrderDetailDAO(BunBoDbContext context)
        {
            _context = context;
        }

        // Lấy chi tiết món ăn trong một đơn hàng
        // Dùng khi cần tính nguyên liệu để trừ kho
        public async Task<List<OrderDetail>> GetByOrderIdAsync(int orderId)
        {
            return await _context.OrderDetails
                .Include(x => x.MenuItem)
                .Where(x => x.OrderId == orderId)
                .ToListAsync();
        }

        // Thêm nhiều dòng chi tiết đơn hàng
        // Dùng khi tạo order có nhiều món
        public async Task AddRangeAsync(List<OrderDetail> orderDetails)
        {
            await _context.OrderDetails.AddRangeAsync(orderDetails);
        }

        // Xóa chi tiết đơn hàng
        // Chỉ nên dùng khi đơn còn Pending
        public void Delete(OrderDetail orderDetail)
        {
            _context.OrderDetails.Remove(orderDetail);
        }
    }
}
