using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.Kitchens
{
    public class CreateKitchenRequest
    {
        // Tên bếp tổng hoặc tên chi nhánh
        [Required(ErrorMessage = "Kitchen name is required.")]
        [MaxLength(100)]
        public string KitchenName { get; set; } = null!;

        // Địa chỉ của bếp hoặc chi nhánh
        [MaxLength(255)]
        public string? Address { get; set; }

        // true: Bếp tổng
        // false: Chi nhánh bán hàng
        public bool IsCentralKitchen { get; set; }
    }
}
