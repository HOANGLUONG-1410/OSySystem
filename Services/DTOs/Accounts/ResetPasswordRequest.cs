using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.Accounts
{
    public class ResetPasswordRequest
    {
        // Mật khẩu mới do Admin thiết lập
        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; } = null!;
    }
}
