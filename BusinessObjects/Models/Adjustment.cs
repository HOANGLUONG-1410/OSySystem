using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Models
{
    public class Adjustment
    {
        public int AdjustmentId { get; set; }

        // Kho được điều chỉnh
        public int KitchenId { get; set; }

        // Nguyên liệu được điều chỉnh
        public int IngredientId { get; set; }

        // Tài khoản đã thực hiện kiểm kê và điều chỉnh
        public int? CreatedByAccountId { get; set; }

        // Số lượng hệ thống trước khi điều chỉnh
        public decimal SystemQuantity { get; set; }

        // Số lượng thực tế do quản lý nhập
        public decimal ActualQuantity { get; set; }

        // ActualQuantity - SystemQuantity
        public decimal DifferenceQuantity { get; set; }

        [MaxLength(500)]
        public string? Reason { get; set; }

        public DateTime CreatedAt { get; set; }
            = DateTime.UtcNow;

        public Kitchen Kitchen { get; set; } = null!;

        public Ingredient Ingredient { get; set; } = null!;

        // Người thực hiện điều chỉnh
        public SystemAccount? CreatedByAccount { get; set; }
    }
}
