using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.Inventories
{
    public class InventoryResponse
    {
        public int InventoryId { get; set; }

        public int KitchenId { get; set; }

        public string KitchenName { get; set; } = null!;

        public int IngredientId { get; set; }

        public string IngredientName { get; set; } = null!;

        public string Unit { get; set; } = null!;

        // Số lượng tồn kho hiện tại
        public decimal Quantity { get; set; }

        // Giá vốn bình quân của nguyên liệu
        public decimal AverageCost { get; set; }

        // Tổng giá trị tồn kho của nguyên liệu này
        public decimal InventoryValue { get; set; }

        public DateTime LastUpdated { get; set; }

        // Cho biết tồn kho đang âm hay không
        public bool IsNegative { get; set; }
    }
}
