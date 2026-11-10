using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Models
{
    public class Ingredient
    {
        public int IngredientId { get; set; }

        [Required]
        [MaxLength(100)]
        public string IngredientName { get; set; } = null!;

        [Required]
        [MaxLength(50)]
        public string Unit { get; set; } = null!;

        public decimal AverageCost { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
        public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
    }
}
