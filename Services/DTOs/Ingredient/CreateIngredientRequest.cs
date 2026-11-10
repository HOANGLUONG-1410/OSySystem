using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.Ingredient
{
    public class CreateIngredientRequest
    {
        [Required]
        [MaxLength(100)]
        public string IngredientName { get; set; } = null!;

        // Ví dụ: kg, g, lít, ml, quả
        [Required]
        [MaxLength(50)]
        public string Unit { get; set; } = null!;

        // Giá vốn bình quân ban đầu
        [Range(0, double.MaxValue)]
        public decimal AverageCost { get; set; }
    }
}
