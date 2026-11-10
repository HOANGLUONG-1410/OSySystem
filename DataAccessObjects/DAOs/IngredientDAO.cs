using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Data;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects.DAOs
{
    public class IngredientDAO
    {
        private readonly BunBoDbContext _context;

        public IngredientDAO(BunBoDbContext context)
        {
            _context = context;
        }

        // Lấy danh sách nguyên liệu còn hoạt động
        // Dùng cho màn hình quản lý nguyên liệu, công thức, nhập kho
        public async Task<List<Ingredient>> GetAllAsync()
        {
            return await _context.Ingredients
                .Where(x => x.IsActive)
                .ToListAsync();
        }

        // Lấy nguyên liệu theo id
        // Dùng khi tạo công thức hoặc ghi nhận biến động kho
        public async Task<Ingredient?> GetByIdAsync(int id)
        {
            return await _context.Ingredients
                .FirstOrDefaultAsync(x => x.IngredientId == id);
        }

        // Thêm nguyên liệu mới
        public async Task AddAsync(Ingredient ingredient)
        {
            await _context.Ingredients.AddAsync(ingredient);
        }

        // Cập nhật nguyên liệu: tên, đơn vị tính, giá vốn bình quân
        public void Update(Ingredient ingredient)
        {
            _context.Ingredients.Update(ingredient);
        }

        // Xóa mềm nguyên liệu để không ảnh hưởng Recipe, InventoryLog cũ
        public void SoftDelete(Ingredient ingredient)
        {
            ingredient.IsActive = false;
            _context.Ingredients.Update(ingredient);
        }
    }
}
