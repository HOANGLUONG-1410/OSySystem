using BusinessObjects.Data;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects.DAOs
{
    public class AdjustmentDAO
    {
        private readonly BunBoDbContext _context;

        public AdjustmentDAO(BunBoDbContext context)
        {
            _context = context;
        }

        // Lấy danh sách phiếu điều chỉnh tồn kho
        // Dùng cho màn hình quản lý sai lệch kho cuối ngày
        public async Task<List<Adjustment>> GetAllAsync()
        {
            return await _context.Adjustments
                .Include(x => x.Kitchen)
                .Include(x => x.Ingredient)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Lấy phiếu điều chỉnh theo chi nhánh
        public async Task<List<Adjustment>> GetByKitchenIdAsync(int kitchenId)
        {
            return await _context.Adjustments
                .Include(x => x.Ingredient)
                .Where(x => x.KitchenId == kitchenId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Thêm phiếu điều chỉnh kho
        // Service sẽ tính DifferenceQuantity trước khi gọi DAO
        public async Task AddAsync(Adjustment adjustment)
        {
            await _context.Adjustments.AddAsync(adjustment);
        }
    }
}
