using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.Kitchens
{
    public class KitchenResponse
    {
        public int KitchenId { get; set; }

        public string KitchenName { get; set; } = null!;

        public string? Address { get; set; }

        public bool IsCentralKitchen { get; set; }

        public bool IsActive { get; set; }
    }
}
