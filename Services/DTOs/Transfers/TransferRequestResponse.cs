using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.Transfers
{
    public class TransferRequestResponse
    {
        // Mã phiếu điều chuyển
        public int TransferRequestId { get; set; }

        // Kitchen xuất nguyên liệu
        public int FromKitchenId { get; set; }

        // Tên Kitchen xuất nguyên liệu
        public string FromKitchenName { get; set; } = string.Empty;

        // Kitchen nhận nguyên liệu
        public int ToKitchenId { get; set; }

        // Tên Kitchen nhận nguyên liệu
        public string ToKitchenName { get; set; } = string.Empty;

        // Trạng thái phiếu:
        // Draft, InTransit, Received hoặc Cancelled
        public string Status { get; set; } = string.Empty;

        // Ghi chú của phiếu điều chuyển
        public string? Note { get; set; }

        // Người tạo phiếu Draft
        public int? CreatedByAccountId { get; set; }

        public string? CreatedByName { get; set; }

        // Người xác nhận xuất hàng khỏi Bếp tổng
        public int? SentByAccountId { get; set; }

        public string? SentByName { get; set; }

        // Người xác nhận nhận hàng tại chi nhánh
        public int? ReceivedByAccountId { get; set; }

        public string? ReceivedByName { get; set; }

        // Thời điểm tạo phiếu
        public DateTime CreatedAt { get; set; }

        // Thời điểm phiếu chuyển từ Draft sang InTransit
        public DateTime? SentAt { get; set; }

        // Thời điểm phiếu chuyển từ InTransit sang Received
        public DateTime? ReceivedAt { get; set; }

        // Danh sách nguyên liệu trong phiếu
        public List<TransferDetailResponse> Items { get; set; } = new();
    }
}
