using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Models
{
    public class Inventory
    {
        public int InventoryId { get; set; }

        public int KitchenId { get; set; }
        public int IngredientId { get; set; }

        public decimal Quantity { get; set; }

        public DateTime LastUpdated { get; set; } = DateTime.Now;

        public Kitchen Kitchen { get; set; } = null!;
        public Ingredient Ingredient { get; set; } = null!;
    }
}
