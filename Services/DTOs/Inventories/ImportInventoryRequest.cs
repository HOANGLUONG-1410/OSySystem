using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.Inventories
{
    public class ImportInventoryRequest
    {
        // Kitchen nhận nguyên liệu nhập kho
        [Range(1, int.MaxValue)]
        public int KitchenId { get; set; }

        // Nguyên liệu được nhập
        [Range(1, int.MaxValue)]
        public int IngredientId { get; set; }

        // Số lượng nhập phải lớn hơn 0
        [Range(
            typeof(decimal),
            "0.001",
            "999999999",
            ErrorMessage = "Import quantity must be greater than 0.")]
        public decimal Quantity { get; set; }

        // Giá nhập trên một đơn vị nguyên liệu
        // Dùng để tính lại giá vốn bình quân
        [Range(
            typeof(decimal),
            "0",
            "999999999")]
        public decimal UnitCost { get; set; }

        // Ghi chú nguồn nhập hoặc nhà cung cấp
        [MaxLength(500)]
        public string? Note { get; set; }
    }
}
