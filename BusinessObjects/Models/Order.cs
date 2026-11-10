using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Enums;

namespace BusinessObjects.Models
{
    public class Order
    {
        public int OrderId { get; set; }

        public int KitchenId { get; set; }

        /*
         * Tài khoản đã tạo đơn.
         *
         * Nullable để:
         * - Không ảnh hưởng dữ liệu cũ.
         * - Cho phép dữ liệu được import từ hệ thống khác.
         */
        public int? CreatedByAccountId { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public decimal TotalAmount { get; set; }

        // Navigation đến Kitchen của đơn hàng
        public Kitchen Kitchen { get; set; } = null!;

        // Navigation đến người tạo đơn
        public SystemAccount? CreatedByAccount { get; set; }

        // Danh sách món thuộc đơn hàng
        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
