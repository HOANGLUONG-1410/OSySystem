using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.Orders
{
    public class CreateOrderRequest
    {
        /*
         * KitchenId của quán tạo đơn.
         * Staff và BranchManager chỉ được dùng KitchenId của chính mình.
         * SystemAdmin có thể tạo cho một Kitchen bất kỳ.
         */
        [Range(1, int.MaxValue)]
        public int KitchenId { get; set; }

        // Một đơn phải có ít nhất một món
        [Required]
        [MinLength(1, ErrorMessage = "Order must contain at least one item.")]
        public List<CreateOrderDetailRequest> Items { get; set; } = new();
    }
}
