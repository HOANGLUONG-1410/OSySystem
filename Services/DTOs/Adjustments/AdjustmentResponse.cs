using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.Adjustments
{
    public class AdjustmentResponse
    {
        public int AdjustmentId { get; set; }

        public int KitchenId { get; set; }

        public string KitchenName { get; set; } = null!;

        public int IngredientId { get; set; }

        public string IngredientName { get; set; } = null!;

        // Audit trail: Ai tạo phiếu điều chỉnh
        public int? CreatedByAccountId { get; set; }

        public string? CreatedByName { get; set; }

        // Số lượng hệ thống trước khi kiểm kê
        public decimal SystemQuantity { get; set; }

        // Số lượng thực tế do quản lý nhập
        public decimal ActualQuantity { get; set; }

        // ActualQuantity - SystemQuantity
        public decimal DifferenceQuantity { get; set; }

        public string? Reason { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
