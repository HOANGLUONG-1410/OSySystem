using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.Recipes
{
    public class CreateRecipeRequest
    {
        // Món ăn được áp dụng công thức
        [Range(1, int.MaxValue)]
        public int MenuItemId { get; set; }

        // Nguyên liệu sử dụng trong món
        [Range(1, int.MaxValue)]
        public int IngredientId { get; set; }

        // Số lượng nguyên liệu cần cho một đơn vị món
        [Range(
            typeof(decimal),
            "0.001",
            "999999999",
            ErrorMessage = "Quantity required must be greater than 0.")]
        public decimal QuantityRequired { get; set; }
    }
}
