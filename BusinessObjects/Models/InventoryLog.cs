using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Enums;

namespace BusinessObjects.Models
{
    public class InventoryLog
    {
        public int InventoryLogId { get; set; }

        public int KitchenId { get; set; }

        public int IngredientId { get; set; }

        /*
         * Người trực tiếp tạo ra biến động kho.
         *
         * Ví dụ:
         * - Staff confirm order.
         * - BranchManager adjustment.
         * - KitchenManager gửi transfer.
         */
        public int? PerformedByAccountId { get; set; }

        public InventoryLogType LogType { get; set; }

        public decimal QuantityChange { get; set; }

        public decimal QuantityBefore { get; set; }

        public decimal QuantityAfter { get; set; }

        [MaxLength(500)]
        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; }
            = DateTime.UtcNow;

        public Kitchen Kitchen { get; set; } = null!;

        public Ingredient Ingredient { get; set; } = null!;

        // Navigation đến tài khoản tạo biến động
        public SystemAccount? PerformedByAccount { get; set; }
    }
}
