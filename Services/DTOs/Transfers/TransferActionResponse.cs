using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.Transfers
{
    public class TransferActionResponse
    {
        public int TransferRequestId { get; set; }

        public string Status { get; set; } = null!;

        public DateTime ActionAt { get; set; }

        /*
         * Danh sách cảnh báo kho âm.
         * Hệ thống vẫn cho xuất hàng theo nguyên tắc đã thiết kế,
         * nhưng ghi warning để quản lý kiểm tra.
         */
        public List<string> Warnings { get; set; } = new();
    }
}
