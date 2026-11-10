using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.Ingredient
{
    public class IngredientResponse
    {
        public int IngredientId { get; set; }

        public string IngredientName { get; set; } = null!;

        public string Unit { get; set; } = null!;

        public decimal AverageCost { get; set; }

        public bool IsActive { get; set; }
    }
}
