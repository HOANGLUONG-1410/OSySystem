using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.DTOs.Auth;

namespace Services.Interfaces
{
    public interface IAuthService
    {
        // Kiểm tra tài khoản, mật khẩu và trả JWT
        Task<LoginResponse> LoginAsync(LoginRequest request);

        // SystemAdmin tạo tài khoản mới
        Task<AccountResponse> CreateAccountAsync(CreateAccountRequest request);
    }
}
