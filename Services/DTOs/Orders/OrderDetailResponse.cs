using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.Orders
{
    public class OrderDetailResponse
    {
        public int OrderDetailId { get; set; }

        public int MenuItemId { get; set; }

        public string MenuItemName { get; set; } = null!;

        public int Quantity { get; set; }

        // Giá bán được chốt tại thời điểm tạo đơn
        public decimal UnitPrice { get; set; }

        // Quantity × UnitPrice
        public decimal SubTotal { get; set; }
    }
}
