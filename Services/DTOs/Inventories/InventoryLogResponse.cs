using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.Inventories
{
    public class InventoryLogResponse
    {
        public int InventoryLogId { get; set; }

        public int KitchenId { get; set; }

        public string KitchenName { get; set; } = null!;

        public int IngredientId { get; set; }

        public string IngredientName { get; set; } = null!;

        // Loại biến động:
        // Import, SaleDeduction, TransferOut, TransferIn, Adjustment, Warning
        public string LogType { get; set; } = null!;

        // Số lượng thay đổi
        // Số dương là cộng kho, số âm là trừ kho
        public decimal QuantityChange { get; set; }

        public decimal QuantityBefore { get; set; }

        public decimal QuantityAfter { get; set; }

        public string? Note { get; set; }

        // Audit trail: Ai thực hiện thao tác này
        public int? PerformedByAccountId { get; set; }

        public string? PerformedByName { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
