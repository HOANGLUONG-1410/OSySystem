using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Data;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects.DAOs
{
    public class OrderDAO
    {
        private readonly BunBoDbContext _context;

        public OrderDAO(BunBoDbContext context)
        {
            _context = context;
        }

        // Lấy toàn bộ đơn hàng
        // Dùng cho màn hình Order Management
        public async Task<List<Order>> GetAllAsync()
        {
            return await _context.Orders
                .AsNoTracking()
                .Include(x => x.Kitchen)
                .Include(x => x.OrderDetails)
                .ThenInclude(x => x.MenuItem)
                .Include(x => x.CreatedByAccount)
                .OrderByDescending(x => x.OrderDate)
                .ToListAsync();
        }

        // Lấy đơn hàng theo id, kèm chi tiết món
        // Dùng khi confirm đơn để lấy OrderDetail
        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _context.Orders
                .AsNoTracking()
                .Include(x => x.OrderDetails)
                .ThenInclude(x => x.MenuItem)
                .Include(x => x.CreatedByAccount)
                .FirstOrDefaultAsync(x => x.OrderId == id);
        }

        // Lấy danh sách đơn Pending
        // Dùng cho màn hình bếp xem đơn cần xác nhận
        public async Task<List<Order>> GetPendingOrdersAsync()
        {
            return await _context.Orders
                .AsNoTracking()
                .Include(x => x.Kitchen)
                .Include(x => x.OrderDetails)
                .ThenInclude(x => x.MenuItem)
                .Include(x => x.CreatedByAccount)
                .Where(x => x.Status == OrderStatus.Pending)
                .OrderBy(x => x.OrderDate)
                .ToListAsync();
        }

        // Thêm đơn hàng mới
        // Khi nhân viên tạo đơn, đơn sẽ ở trạng thái Pending
        public async Task AddAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
        }

        // Cập nhật đơn hàng
        // Dùng khi đổi trạng thái Pending -> Confirmed hoặc Cancelled
        public void Update(Order order)
        {
            _context.Orders.Update(order);
        }

        // Lấy tất cả đơn hàng thuộc một Kitchen
        // Dùng cho BranchManager hoặc Staff xem đơn tại quán mình
        public async Task<List<Order>> GetByKitchenIdAsync(int kitchenId)
        {
            return await _context.Orders
                .AsNoTracking()
                .Include(x => x.Kitchen)
                .Include(x => x.OrderDetails)
                    .ThenInclude(x => x.MenuItem)
                .Include(x => x.CreatedByAccount)
                .Where(x => x.KitchenId == kitchenId)
                .OrderByDescending(x => x.OrderDate)
                .ToListAsync();
        }

        // Lấy đơn hàng theo Kitchen và trạng thái
        // Dùng cho màn hình bếp lọc các đơn Pending, Confirmed hoặc Completed
        public async Task<List<Order>> GetByKitchenAndStatusAsync(
            int kitchenId,
            OrderStatus status)
        {
            return await _context.Orders
                .AsNoTracking()
                .Include(x => x.Kitchen)
                .Include(x => x.OrderDetails)
                    .ThenInclude(x => x.MenuItem)
                .Include(x => x.CreatedByAccount)
                .Where(x =>
                    x.KitchenId == kitchenId &&
                    x.Status == status)
                .OrderBy(x => x.OrderDate)
                .ToListAsync();
        }
    }
}
