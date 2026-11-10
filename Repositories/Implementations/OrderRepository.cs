using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using DataAccessObjects.DAOs;
using Repositories.Interfaces;

namespace Repositories.Implementations
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderDAO _orderDAO;

        public OrderRepository(OrderDAO orderDAO)
        {
            _orderDAO = orderDAO;
        }

        // Chuyển yêu cầu lấy toàn bộ đơn xuống DAO
        public Task<List<Order>> GetAllAsync()
        {
            return _orderDAO.GetAllAsync();
        }

        // Lấy một đơn và toàn bộ món trong đơn
        public Task<Order?> GetByIdAsync(int orderId)
        {
            return _orderDAO.GetByIdAsync(orderId);
        }

        // Lấy toàn bộ đơn Pending
        public Task<List<Order>> GetPendingOrdersAsync()
        {
            return _orderDAO.GetPendingOrdersAsync();
        }

        // Lấy đơn của một chi nhánh
        public Task<List<Order>> GetByKitchenIdAsync(int kitchenId)
        {
            return _orderDAO.GetByKitchenIdAsync(kitchenId);
        }

        // Lấy đơn theo chi nhánh và trạng thái
        public Task<List<Order>> GetByKitchenAndStatusAsync(
            int kitchenId,
            OrderStatus status)
        {
            return _orderDAO.GetByKitchenAndStatusAsync(
                kitchenId,
                status);
        }

        // Thêm Order vào Change Tracker
        // Các OrderDetail nằm trong navigation collection cũng sẽ được thêm
        public Task AddAsync(Order order)
        {
            return _orderDAO.AddAsync(order);
        }

        // Đánh dấu Order đã thay đổi
        public void Update(Order order)
        {
            _orderDAO.Update(order);
        }
    }
}
