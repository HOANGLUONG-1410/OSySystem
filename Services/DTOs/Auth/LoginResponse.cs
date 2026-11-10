using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTOs.Auth
{
    public class LoginResponse
    {
        // JWT trả về cho client
        public string AccessToken { get; set; } = null!;

        // Thời điểm token hết hạn
        public DateTime ExpiresAt { get; set; }

        public int AccountId { get; set; }

        public string Username { get; set; } = null!;

        public string FullName { get; set; } = null!;

        public string Role { get; set; } = null!;

        // Admin có thể không thuộc Kitchen cụ thể nên nullable
        public int? KitchenId { get; set; }

        public string? KitchenName { get; set; }
    }
}
