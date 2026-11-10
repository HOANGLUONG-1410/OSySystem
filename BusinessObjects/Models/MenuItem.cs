using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Models
{
    public class MenuItem
    {
        public int MenuItemId { get; set; }

        [Required]
        [MaxLength(100)]
        public string MenuItemName { get; set; } = null!;

        public decimal SellingPrice { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
