using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.Orders
{
    public class CreateOrderDetailRequest
    {
        // Món được khách gọi
        [Range(1, int.MaxValue)]
        public int MenuItemId { get; set; }

        // Số lượng món phải lớn hơn 0
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
