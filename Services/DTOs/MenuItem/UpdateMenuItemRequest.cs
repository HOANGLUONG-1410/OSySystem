using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.MenuItem
{
    public class UpdateMenuItemRequest
    {
        [Required]
        [MaxLength(100)]
        public string MenuItemName { get; set; } = null!;

        [Range(1, double.MaxValue)]
        public decimal SellingPrice { get; set; }
    }
}
