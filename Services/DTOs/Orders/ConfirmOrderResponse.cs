using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.Orders
{
    public class ConfirmOrderResponse
    {
        public int OrderId { get; set; }

        public string Status { get; set; } = null!;

        public DateTime ConfirmedAt { get; set; }

        // Danh sách nguyên liệu bị âm sau khi trừ kho
        // Hệ thống vẫn confirm đơn nhưng trả cảnh báo cho người dùng
        public List<string> Warnings { get; set; } = new();
    }
}
