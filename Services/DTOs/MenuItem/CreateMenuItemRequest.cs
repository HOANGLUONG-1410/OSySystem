using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.MenuItem
{
    public class CreateMenuItemRequest
    {
        [Required]
        [MaxLength(100)]
        public string MenuItemName { get; set; } = null!;

        // Giá bán phải lớn hơn 0
        [Range(1, double.MaxValue)]
        public decimal SellingPrice { get; set; }
    }
}
