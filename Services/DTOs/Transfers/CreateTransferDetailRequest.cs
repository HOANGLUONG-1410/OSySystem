using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.Transfers
{
    public class CreateTransferDetailRequest
    {
        // Nguyên liệu cần chuyển
        [Range(1, int.MaxValue)]
        public int IngredientId { get; set; }

        // Số lượng chuyển phải lớn hơn 0
        [Range(
            typeof(decimal),
            "0.001",
            "999999999",
            ErrorMessage =
                "Transfer quantity must be greater than 0.")]
        public decimal Quantity { get; set; }
    }
}
