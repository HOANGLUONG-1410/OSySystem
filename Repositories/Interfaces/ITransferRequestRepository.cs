using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Enums;
using BusinessObjects.Models;

namespace Repositories.Interfaces
{
    public interface ITransferRequestRepository
    {
        // Lấy toàn bộ phiếu điều chuyển
        Task<List<TransferRequest>> GetAllAsync();

        // Lấy một phiếu theo id
        Task<TransferRequest?> GetByIdAsync(int transferRequestId);

        // Lấy các phiếu xuất phát từ một Kitchen
        Task<List<TransferRequest>> GetByFromKitchenIdAsync(
            int kitchenId);

        // Lấy các phiếu được gửi đến một Kitchen
        Task<List<TransferRequest>> GetByToKitchenIdAsync(
            int kitchenId);

        // Lấy phiếu theo Kitchen nhận và trạng thái
        Task<List<TransferRequest>> GetByToKitchenAndStatusAsync(
            int kitchenId,
            TransferStatus status);

        // Thêm phiếu mới
        Task AddAsync(TransferRequest transferRequest);

        // Cập nhật phiếu
        void Update(TransferRequest transferRequest);
    }
}
