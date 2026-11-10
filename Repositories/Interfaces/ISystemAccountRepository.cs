using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Enums;
using BusinessObjects.Models;


namespace Repositories.Interfaces
{
    public interface ISystemAccountRepository
    {
        // Lấy toàn bộ tài khoản để Admin quản lý
        Task<List<SystemAccount>> GetAllAsync();

        // Lấy tài khoản đang hoạt động
        Task<List<SystemAccount>> GetActiveAccountsAsync();

        // Lấy một tài khoản theo khóa chính
        Task<SystemAccount?> GetByIdAsync(int accountId);

        // Lấy tài khoản theo username, chủ yếu dùng khi đăng nhập
        Task<SystemAccount?> GetByUsernameAsync(string username);

        // Kiểm tra username đã tồn tại hay chưa
        Task<bool> IsUsernameExistsAsync(string username);

        // Lấy tài khoản thuộc một bếp hoặc chi nhánh
        Task<List<SystemAccount>> GetByKitchenIdAsync(int kitchenId);

        // Lấy danh sách tài khoản theo role
        Task<List<SystemAccount>> GetByRoleAsync(AccountRole role);

        // Thêm tài khoản mới vào Change Tracker
        Task AddAsync(SystemAccount account);

        // Cập nhật tài khoản
        void Update(SystemAccount account);

        // Khóa tài khoản
        void Deactivate(SystemAccount account);

        // Mở khóa tài khoản
        void Activate(SystemAccount account);
    }
}
