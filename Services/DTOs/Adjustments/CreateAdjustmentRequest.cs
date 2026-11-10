using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.Adjustments
{
    public class CreateAdjustmentRequest
    {
        // Kitchen cần kiểm kê và điều chỉnh
        [Range(1, int.MaxValue)]
        public int KitchenId { get; set; }

        // Nguyên liệu được kiểm kê
        [Range(1, int.MaxValue)]
        public int IngredientId { get; set; }

        // Số lượng thực tế đếm được tại kho
        [Range(
            typeof(decimal),
            "0",
            "999999999",
            ErrorMessage = "Actual quantity cannot be negative.")]
        public decimal ActualQuantity { get; set; }

        // Lý do sai lệch:
        // hư hỏng, rơi vãi, kiểm kê cuối ngày...
        [Required]
        [MaxLength(500)]
        public string Reason { get; set; } = null!;
    }
}
