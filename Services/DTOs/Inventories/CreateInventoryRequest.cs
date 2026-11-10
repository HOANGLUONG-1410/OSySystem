using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.Inventories
{
    public class CreateInventoryRequest
    {
        // Kitchen cần khởi tạo dòng tồn kho
        [Range(1, int.MaxValue)]
        public int KitchenId { get; set; }

        // Nguyên liệu cần thêm vào kho của Kitchen
        [Range(1, int.MaxValue)]
        public int IngredientId { get; set; }

        // Số lượng tồn kho ban đầu
        [Range(
            typeof(decimal),
            "0",
            "999999999",
            ErrorMessage = "Initial quantity cannot be negative.")]
        public decimal InitialQuantity { get; set; }
    }
}
