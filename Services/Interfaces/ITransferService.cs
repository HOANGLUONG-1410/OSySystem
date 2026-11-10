using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Enums;
using Services.DTOs.Transfers;

namespace Services.Interfaces
{
    public interface ITransferService
    {
        // Lấy các phiếu mà người dùng hiện tại được phép xem
        Task<List<TransferRequestResponse>>
            GetAccessibleTransfersAsync();

        // Lấy các phiếu theo trạng thái trong phạm vi quyền
        Task<List<TransferRequestResponse>>
            GetByStatusAsync(TransferStatus status);

        // Lấy chi tiết một phiếu
        Task<TransferRequestResponse> GetByIdAsync(
            int transferRequestId);

        // Tạo phiếu Draft
        Task<TransferRequestResponse> CreateAsync(
            CreateTransferRequest request);

        // Duyệt xuất hàng: Draft -> InTransit
        Task<TransferActionResponse> SendAsync(
            int transferRequestId);

        // Xác nhận nhận hàng: InTransit -> Received
        Task<TransferActionResponse> ReceiveAsync(
            int transferRequestId);

        // Hủy phiếu khi còn Draft
        Task<TransferRequestResponse> CancelAsync(
            int transferRequestId);
    }
}
