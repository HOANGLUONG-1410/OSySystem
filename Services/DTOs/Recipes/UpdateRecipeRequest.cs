using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.Recipes
{
    public class UpdateRecipeRequest
    {
        // Khi cập nhật Recipe chỉ thay đổi định lượng
        // Không thay MenuItemId hoặc IngredientId
        [Range(
            typeof(decimal),
            "0.001",
            "999999999")]
        public decimal QuantityRequired { get; set; }
    }
}
