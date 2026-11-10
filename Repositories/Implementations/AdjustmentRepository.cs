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
    public class AdjustmentRepository : IAdjustmentRepository
    {
        private readonly AdjustmentDAO _adjustmentDAO;

        public AdjustmentRepository(AdjustmentDAO adjustmentDAO)
        {
            _adjustmentDAO = adjustmentDAO;
        }

        // Chuyển yêu cầu lấy toàn bộ phiếu điều chỉnh xuống DAO
        public Task<List<Adjustment>> GetAllAsync()
        {
            return _adjustmentDAO.GetAllAsync();
        }

        // Lấy phiếu điều chỉnh của một Kitchen cụ thể
        public Task<List<Adjustment>> GetByKitchenIdAsync(int kitchenId)
        {
            return _adjustmentDAO.GetByKitchenIdAsync(kitchenId);
        }

        // Thêm phiếu điều chỉnh vào Change Tracker
        // UnitOfWork sẽ lưu dữ liệu xuống database
        public Task AddAsync(Adjustment adjustment)
        {
            return _adjustmentDAO.AddAsync(adjustment);
        }
    }
}
