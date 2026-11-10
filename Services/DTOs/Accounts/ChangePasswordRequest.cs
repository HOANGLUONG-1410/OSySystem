using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.Accounts
{
    public class ChangePasswordRequest
    {
        // Mật khẩu hiện tại để xác minh đúng chủ tài khoản
        [Required]
        public string CurrentPassword { get; set; } = null!;

        // Mật khẩu mới
        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; } = null!;
    }
}
