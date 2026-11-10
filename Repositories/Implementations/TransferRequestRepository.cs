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
    public class TransferRequestRepository
        : ITransferRequestRepository
    {
        private readonly TransferRequestDAO _transferRequestDAO;

        public TransferRequestRepository(
            TransferRequestDAO transferRequestDAO)
        {
            _transferRequestDAO = transferRequestDAO;
        }

        // Lấy toàn bộ phiếu điều chuyển
        public Task<List<TransferRequest>> GetAllAsync()
        {
            return _transferRequestDAO.GetAllAsync();
        }

        // Lấy một phiếu kèm chi tiết nguyên liệu
        public Task<TransferRequest?> GetByIdAsync(
            int transferRequestId)
        {
            return _transferRequestDAO
                .GetByIdAsync(transferRequestId);
        }

        // Lấy phiếu do một Kitchen tạo
        public Task<List<TransferRequest>> GetByFromKitchenIdAsync(
            int kitchenId)
        {
            return _transferRequestDAO
                .GetByFromKitchenIdAsync(kitchenId);
        }

        // Lấy phiếu gửi đến một Kitchen
        public Task<List<TransferRequest>> GetByToKitchenIdAsync(
            int kitchenId)
        {
            return _transferRequestDAO
                .GetByToKitchenIdAsync(kitchenId);
        }

        // Lấy phiếu đang ở trạng thái cụ thể
        public Task<List<TransferRequest>>
            GetByToKitchenAndStatusAsync(
                int kitchenId,
                TransferStatus status)
        {
            return _transferRequestDAO
                .GetByToKitchenAndStatusAsync(
                    kitchenId,
                    status);
        }

        // Thêm phiếu vào Change Tracker
        public Task AddAsync(
            TransferRequest transferRequest)
        {
            return _transferRequestDAO.AddAsync(
                transferRequest);
        }

        // Đánh dấu phiếu đã thay đổi
        public void Update(
            TransferRequest transferRequest)
        {
            _transferRequestDAO.Update(
                transferRequest);
        }
    }
}
