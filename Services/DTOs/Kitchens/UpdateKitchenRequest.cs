using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.Kitchens
{
    public class UpdateKitchenRequest
    {
        // Tên mới của bếp hoặc chi nhánh
        [Required(ErrorMessage = "Kitchen name is required.")]
        [MaxLength(100)]
        public string KitchenName { get; set; } = null!;

        // Địa chỉ mới
        [MaxLength(255)]
        public string? Address { get; set; }
    }
}
