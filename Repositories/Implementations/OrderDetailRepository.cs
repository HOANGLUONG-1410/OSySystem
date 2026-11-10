using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;
using DataAccessObjects.DAOs;
using Repositories.Interfaces;

namespace Repositories.Implementations
{
    public class OrderDetailRepository : IOrderDetailRepository
    {
        private readonly OrderDetailDAO _orderDetailDAO;

        public OrderDetailRepository(OrderDetailDAO orderDetailDAO)
        {
            _orderDetailDAO = orderDetailDAO;
        }

        // Lấy các món trong một đơn
        public Task<List<OrderDetail>> GetByOrderIdAsync(int orderId)
        {
            return _orderDetailDAO.GetByOrderIdAsync(orderId);
        }

        // Thêm nhiều OrderDetail cùng lúc
        public Task AddRangeAsync(List<OrderDetail> orderDetails)
        {
            return _orderDetailDAO.AddRangeAsync(orderDetails);
        }

        // Xóa một món khỏi đơn Pending
        public void Delete(OrderDetail orderDetail)
        {
            _orderDetailDAO.Delete(orderDetail);
        }
    }
}
