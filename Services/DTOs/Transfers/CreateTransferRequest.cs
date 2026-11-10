using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.Transfers
{
    public class CreateTransferRequest
    {
        /*
         * Bếp xuất hàng.
         * Trong nghiệp vụ hiện tại phải là Bếp tổng.
         */
        [Range(1, int.MaxValue)]
        public int FromKitchenId { get; set; }

        // Chi nhánh nhận hàng
        [Range(1, int.MaxValue)]
        public int ToKitchenId { get; set; }

        // Ghi chú cho chuyến hàng
        [MaxLength(500)]
        public string? Note { get; set; }

        // Danh sách nguyên liệu được chuyển
        [Required]
        [MinLength(
            1,
            ErrorMessage =
                "Transfer request must contain at least one ingredient.")]
        public List<CreateTransferDetailRequest> Items { get; set; }
            = new();
    }
}
