using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Enums;

namespace Services.DTOs.Accounts
{
    public class UpdateAccountRequest
    {
        // Họ tên mới của tài khoản
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = null!;

        // Role mới của tài khoản
        public AccountRole Role { get; set; }

        // Kitchen mới mà tài khoản trực thuộc
        public int? KitchenId { get; set; }
    }
}
