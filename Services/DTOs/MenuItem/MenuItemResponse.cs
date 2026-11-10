using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.MenuItem
{
    public class MenuItemResponse
    {
        public int MenuItemId { get; set; }

        public string MenuItemName { get; set; } = null!;

        public decimal SellingPrice { get; set; }

        public bool IsActive { get; set; }
    }
}
