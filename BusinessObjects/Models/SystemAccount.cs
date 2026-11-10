using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Enums;

namespace BusinessObjects.Models
{
    public class SystemAccount
    {
        public int AccountId { get; set; }

        // Tên đăng nhập dùng để login
        // Ví dụ: admin, manager_a, staff_a
        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = null!;

        // Mật khẩu đã hash, không lưu mật khẩu plain text
        // Ví dụ sau này dùng BCrypt để hash password
        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; } = null!;

        // Họ tên người dùng để hiển thị trên giao diện
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = null!;

        // Role quyết định người dùng được vào API nào
        public AccountRole Role { get; set; }

        // KitchenId xác định người dùng thuộc bếp/quán nào
        // SystemAdmin có thể để null vì quản lý toàn hệ thống
        public int? KitchenId { get; set; }

        // Dùng để khóa tài khoản thay vì xóa khỏi database
        public bool IsActive { get; set; } = true;

        // Ngày tạo tài khoản
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation property
        public Kitchen? Kitchen { get; set; }
    }
}
