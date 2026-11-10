using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Models
{
    public class Recipe
    {
        public int RecipeId { get; set; }

        public int MenuItemId { get; set; }
        public int IngredientId { get; set; }

        public decimal QuantityRequired { get; set; }

        public MenuItem MenuItem { get; set; } = null!;
        public Ingredient Ingredient { get; set; } = null!;
    }
}
