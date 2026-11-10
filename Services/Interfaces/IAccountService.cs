using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.DTOs.Accounts;
using Services.DTOs.Auth;

namespace Services.Interfaces
{
    public interface IAccountService
    {
        // Admin lấy danh sách toàn bộ tài khoản
        Task<List<AccountResponse>> GetAllAsync();

        // BranchManager lấy danh sách tài khoản trong chi nhánh
        Task<List<AccountResponse>> GetByKitchenIdAsync(int kitchenId);

        // Lấy thông tin một tài khoản
        Task<AccountResponse> GetByIdAsync(int accountId);

        // Admin cập nhật role, họ tên và Kitchen
        Task<AccountResponse> UpdateAsync(
            int accountId,
            UpdateAccountRequest request);

        // Khóa tài khoản
        Task DeactivateAsync(int accountId);

        // Mở khóa tài khoản
        Task ActivateAsync(int accountId);

        // Người dùng tự đổi mật khẩu
        Task ChangePasswordAsync(
            int accountId,
            ChangePasswordRequest request);

        // Admin reset mật khẩu cho tài khoản khác
        Task ResetPasswordAsync(
            int accountId,
            ResetPasswordRequest request);
    }
}
