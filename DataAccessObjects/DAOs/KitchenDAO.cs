using BusinessObjects.Data;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects.DAOs
{
    public class KitchenDAO
    {
        private readonly BunBoDbContext _context;

        public KitchenDAO(BunBoDbContext context)
        {
            _context = context;
        }

        // Lấy toàn bộ danh sách bếp/quán để hiển thị trên Dashboard hoặc form chọn chi nhánh
        public async Task<List<Kitchen>> GetAllAsync()
        {
            return await _context.Kitchens
                .Where(x => x.IsActive)
                .ToListAsync();
        }

        // Lấy thông tin một bếp/quán theo id
        // Dùng khi tạo đơn hàng, điều chỉnh kho, điều chuyển kho
        public async Task<Kitchen?> GetByIdAsync(int id)
        {
            return await _context.Kitchens
                .FirstOrDefaultAsync(x => x.KitchenId == id);
        }

        // Thêm bếp/quán mới
        public async Task AddAsync(Kitchen kitchen)
        {
            await _context.Kitchens.AddAsync(kitchen);
        }

        // Cập nhật thông tin bếp/quán
        public void Update(Kitchen kitchen)
        {
            _context.Kitchens.Update(kitchen);
        }

        // Xóa mềm bếp/quán, không xóa khỏi database để tránh mất lịch sử đơn hàng/kho
        public void SoftDelete(Kitchen kitchen)
        {
            kitchen.IsActive = false;
            _context.Kitchens.Update(kitchen);
        }
    }
}
