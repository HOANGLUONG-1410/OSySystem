using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.Recipes
{
    public class RecipeResponse
    {
        public int RecipeId { get; set; }

        public int MenuItemId { get; set; }

        public string MenuItemName { get; set; } = null!;

        public int IngredientId { get; set; }

        public string IngredientName { get; set; } = null!;

        public string IngredientUnit { get; set; } = null!;

        public decimal QuantityRequired { get; set; }
    }
}
