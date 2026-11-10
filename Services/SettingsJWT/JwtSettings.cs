using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.SettingsJWT
{
    public class JwtSettings
    {
        // Tên section trong appsettings.json
        public const string SectionName = "JwtSettings";

        // Secret key dùng để ký token
        public string SecretKey { get; set; } = null!;

        // Đơn vị phát hành token
        public string Issuer { get; set; } = null!;

        // Đối tượng sử dụng token
        public string Audience { get; set; } = null!;

        // Thời gian sống của token
        public int ExpirationMinutes { get; set; }
    }
}
