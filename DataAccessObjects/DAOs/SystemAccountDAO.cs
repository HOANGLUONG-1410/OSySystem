using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Data;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects.DAOs
{
    public class SystemAccountDAO
    {
        private readonly BunBoDbContext _context;

        public SystemAccountDAO(BunBoDbContext context)
        {
            _context = context;
        }

        // Lấy toàn bộ tài khoản trong hệ thống
        // Dùng cho màn hình Admin quản lý user
        public async Task<List<SystemAccount>> GetAllAsync()
        {
            return await _context.SystemAccounts
                .Include(x => x.Kitchen)
                .OrderBy(x => x.Role)
                .ThenBy(x => x.Username)
                .ToListAsync();
        }

        // Lấy tài khoản đang hoạt động
        // Dùng cho màn hình quản lý tài khoản, tránh hiển thị tài khoản đã khóa nếu không cần
        public async Task<List<SystemAccount>> GetActiveAccountsAsync()
        {
            return await _context.SystemAccounts
                .Include(x => x.Kitchen)
                .Where(x => x.IsActive)
                .OrderBy(x => x.Role)
                .ThenBy(x => x.Username)
                .ToListAsync();
        }

        // Lấy tài khoản theo id
        // Dùng khi xem chi tiết, sửa thông tin, đổi role, khóa tài khoản
        public async Task<SystemAccount?> GetByIdAsync(int accountId)
        {
            return await _context.SystemAccounts
                .Include(x => x.Kitchen)
                .FirstOrDefaultAsync(x => x.AccountId == accountId);
        }

        // Lấy tài khoản theo username
        // Dùng chính trong chức năng đăng nhập
        public async Task<SystemAccount?> GetByUsernameAsync(string username)
        {
            return await _context.SystemAccounts
                .Include(x => x.Kitchen)
                .FirstOrDefaultAsync(x => x.Username == username);
        }

        // Kiểm tra username đã tồn tại chưa
        // Dùng khi Admin tạo tài khoản mới để tránh trùng username
        public async Task<bool> IsUsernameExistsAsync(string username)
        {
            return await _context.SystemAccounts
                .AnyAsync(x => x.Username == username);
        }

        // Lấy danh sách tài khoản thuộc một chi nhánh/bếp
        // Dùng khi Admin hoặc Manager muốn xem nhân sự của một Kitchen cụ thể
        public async Task<List<SystemAccount>> GetByKitchenIdAsync(int kitchenId)
        {
            return await _context.SystemAccounts
                .Include(x => x.Kitchen)
                .Where(x => x.KitchenId == kitchenId)
                .OrderBy(x => x.Role)
                .ThenBy(x => x.Username)
                .ToListAsync();
        }

        // Lấy danh sách tài khoản theo role
        // Ví dụ: lấy toàn bộ BranchManager hoặc Staff
        public async Task<List<SystemAccount>> GetByRoleAsync(AccountRole role)
        {
            return await _context.SystemAccounts
                .Include(x => x.Kitchen)
                .Where(x => x.Role == role)
                .OrderBy(x => x.Username)
                .ToListAsync();
        }

        // Thêm tài khoản mới
        // Service sẽ hash password trước, DAO chỉ lưu xuống database
        public async Task AddAsync(SystemAccount account)
        {
            await _context.SystemAccounts.AddAsync(account);
        }

        // Cập nhật thông tin tài khoản
        // Dùng khi sửa FullName, Role, KitchenId, trạng thái tài khoản
        public void Update(SystemAccount account)
        {
            _context.SystemAccounts.Update(account);
        }

        // Khóa tài khoản thay vì xóa vật lý
        // Làm vậy để tránh mất lịch sử thao tác sau này nếu có audit user
        public void Deactivate(SystemAccount account)
        {
            account.IsActive = false;
            _context.SystemAccounts.Update(account);
        }

        // Mở khóa tài khoản
        // Dùng khi Admin muốn cho tài khoản hoạt động trở lại
        public void Activate(SystemAccount account)
        {
            account.IsActive = true;
            _context.SystemAccounts.Update(account);
        }
    }
}
