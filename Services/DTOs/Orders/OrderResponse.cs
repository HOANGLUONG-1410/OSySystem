using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.Orders
{
    public class OrderResponse
    {
        public int OrderId { get; set; }

        public int KitchenId { get; set; }

        public string KitchenName { get; set; } = null!;

        // Audit trail: Ai tạo đơn này
        public int? CreatedByAccountId { get; set; }

        public string? CreatedByName { get; set; }

        public DateTime OrderDate { get; set; }

        public string Status { get; set; } = null!;

        public decimal TotalAmount { get; set; }

        public List<OrderDetailResponse> OrderDetails { get; set; } = new();
    }
}
