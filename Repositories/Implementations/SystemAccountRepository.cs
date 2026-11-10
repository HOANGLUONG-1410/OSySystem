using BusinessObjects.Enums;
using BusinessObjects.Models;
using DataAccessObjects.DAOs;
using Repositories.Interfaces;

namespace Repositories.Implementations
{
    public class SystemAccountRepository : ISystemAccountRepository
    {
        private readonly SystemAccountDAO _systemAccountDAO;

        public SystemAccountRepository(SystemAccountDAO systemAccountDAO)
        {
            _systemAccountDAO = systemAccountDAO;
        }

        // Repository chuyển yêu cầu lấy danh sách tài khoản xuống DAO
        public Task<List<SystemAccount>> GetAllAsync()
        {
            return _systemAccountDAO.GetAllAsync();
        }

        // Lấy các tài khoản chưa bị khóa
        public Task<List<SystemAccount>> GetActiveAccountsAsync()
        {
            return _systemAccountDAO.GetActiveAccountsAsync();
        }

        // Tìm tài khoản bằng AccountId
        public Task<SystemAccount?> GetByIdAsync(int accountId)
        {
            return _systemAccountDAO.GetByIdAsync(accountId);
        }

        // Tìm tài khoản bằng username để AuthService kiểm tra đăng nhập
        public Task<SystemAccount?> GetByUsernameAsync(string username)
        {
            return _systemAccountDAO.GetByUsernameAsync(username);
        }

        // Kiểm tra trùng username khi tạo tài khoản mới
        public Task<bool> IsUsernameExistsAsync(string username)
        {
            return _systemAccountDAO.IsUsernameExistsAsync(username);
        }

        // Lấy nhân viên và quản lý thuộc một Kitchen
        public Task<List<SystemAccount>> GetByKitchenIdAsync(int kitchenId)
        {
            return _systemAccountDAO.GetByKitchenIdAsync(kitchenId);
        }

        // Lấy tài khoản theo vai trò
        public Task<List<SystemAccount>> GetByRoleAsync(AccountRole role)
        {
            return _systemAccountDAO.GetByRoleAsync(role);
        }

        // Repository không SaveChanges trực tiếp
        // Service sẽ quyết định thời điểm lưu qua UnitOfWork
        public Task AddAsync(SystemAccount account)
        {
            return _systemAccountDAO.AddAsync(account);
        }

        // Đưa thay đổi của tài khoản vào Change Tracker
        public void Update(SystemAccount account)
        {
            _systemAccountDAO.Update(account);
        }

        // Khóa tài khoản nhưng vẫn giữ dữ liệu lịch sử
        public void Deactivate(SystemAccount account)
        {
            _systemAccountDAO.Deactivate(account);
        }

        // Mở lại tài khoản đã bị khóa
        public void Activate(SystemAccount account)
        {
            _systemAccountDAO.Activate(account);
        }
    }
}
