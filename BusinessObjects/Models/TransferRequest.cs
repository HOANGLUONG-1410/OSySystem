using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Enums;

namespace BusinessObjects.Models
{
    public class TransferRequest
    {
        public int TransferRequestId { get; set; }

        public int FromKitchenId { get; set; }

        public int ToKitchenId { get; set; }

        // Tài khoản tạo phiếu điều chuyển
        public int? CreatedByAccountId { get; set; }

        // Tài khoản xác nhận xuất hàng
        public int? SentByAccountId { get; set; }

        // Tài khoản xác nhận nhận hàng
        public int? ReceivedByAccountId { get; set; }

        public TransferStatus Status { get; set; }
            = TransferStatus.Draft;

        [MaxLength(500)]
        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; }
            = DateTime.UtcNow;

        public DateTime? SentAt { get; set; }

        public DateTime? ReceivedAt { get; set; }

        public Kitchen FromKitchen { get; set; } = null!;

        public Kitchen ToKitchen { get; set; } = null!;

        public SystemAccount? CreatedByAccount { get; set; }

        public SystemAccount? SentByAccount { get; set; }

        public SystemAccount? ReceivedByAccount { get; set; }

        public ICollection<TransferRequestDetail>
            TransferRequestDetails { get; set; }
                = new List<TransferRequestDetail>();
    }
}
