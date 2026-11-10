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
    public class KitchenRepository : IKitchenRepository
    {
        private readonly KitchenDAO _kitchenDAO;

        public KitchenRepository(KitchenDAO kitchenDAO)
        {
            _kitchenDAO = kitchenDAO;
        }

        // Chuyển yêu cầu lấy danh sách Kitchen xuống DAO
        public Task<List<Kitchen>> GetAllAsync()
        {
            return _kitchenDAO.GetAllAsync();
        }

        // Tìm một Kitchen theo khóa chính
        public Task<Kitchen?> GetByIdAsync(int kitchenId)
        {
            return _kitchenDAO.GetByIdAsync(kitchenId);
        }

        // Thêm Kitchen vào Change Tracker
        // UnitOfWork sẽ lưu xuống database sau
        public Task AddAsync(Kitchen kitchen)
        {
            return _kitchenDAO.AddAsync(kitchen);
        }

        // Đưa thay đổi Kitchen vào Change Tracker
        public void Update(Kitchen kitchen)
        {
            _kitchenDAO.Update(kitchen);
        }

        // Đánh dấu Kitchen không còn hoạt động
        public void SoftDelete(Kitchen kitchen)
        {
            _kitchenDAO.SoftDelete(kitchen);
        }
    }
}
