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
    public class RecipeDAO
    {
        private readonly BunBoDbContext _context;

        public RecipeDAO(BunBoDbContext context)
        {
            _context = context;
        }

        // Lấy công thức của một món ăn
        // Dùng khi bếp confirm đơn để tính số nguyên liệu cần trừ
        public async Task<List<Recipe>> GetByMenuItemIdAsync(int menuItemId)
        {
            return await _context.Recipes
                .Include(x => x.Ingredient)
                .Where(x => x.MenuItemId == menuItemId)
                .ToListAsync();
        }

        // Lấy toàn bộ công thức
        // Dùng cho màn hình quản lý Recipe
        public async Task<List<Recipe>> GetAllAsync()
        {
            return await _context.Recipes
                .Include(x => x.MenuItem)
                .Include(x => x.Ingredient)
                .ToListAsync();
        }

        // Thêm dòng công thức
        // Ví dụ: 1 tô bún bò cần 0.2kg thịt bò
        public async Task AddAsync(Recipe recipe)
        {
            await _context.Recipes.AddAsync(recipe);
        }

        // Cập nhật định lượng nguyên liệu trong món
        public void Update(Recipe recipe)
        {
            _context.Recipes.Update(recipe);
        }

        // Xóa một nguyên liệu khỏi công thức món ăn
        public void Delete(Recipe recipe)
        {
            _context.Recipes.Remove(recipe);
        }

        // Lấy một dòng công thức cụ thể
        public async Task<Recipe?> GetByIdAsync(int id)
        {
            return await _context.Recipes
                .FirstOrDefaultAsync(x => x.RecipeId == id);
        }
    }
}
