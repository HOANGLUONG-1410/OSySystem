using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Models
{
    public class Kitchen
    {
        public int KitchenId { get; set; }

        [Required]
        [MaxLength(100)]
        public string KitchenName { get; set; } = null!;

        [MaxLength(255)]
        public string? Address { get; set; }

        public bool IsCentralKitchen { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();

        public ICollection<Order> Orders { get; set; } = new List<Order>();

        // Một bếp/quán có nhiều tài khoản nhân viên/quản lý
        public ICollection<SystemAccount> SystemAccounts { get; set; } = new List<SystemAccount>();
    }
}
