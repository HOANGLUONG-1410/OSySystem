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
    public class MenuItemDAO
    {
        private readonly BunBoDbContext _context;

        public MenuItemDAO(BunBoDbContext context)
        {
            _context = context;
        }

        // Lấy danh sách món ăn đang bán
        // Dùng cho màn hình tạo đơn hàng
        public async Task<List<MenuItem>> GetAllAsync()
        {
            return await _context.MenuItems
                .Where(x => x.IsActive)
                .ToListAsync();
        }

        // Lấy món ăn theo id
        // Dùng để lấy giá bán khi tạo OrderDetail
        public async Task<MenuItem?> GetByIdAsync(int id)
        {
            return await _context.MenuItems
                .FirstOrDefaultAsync(x => x.MenuItemId == id);
        }

        // Lấy món ăn kèm công thức
        // Dùng khi confirm đơn hàng để biết món này cần nguyên liệu nào
        public async Task<MenuItem?> GetWithRecipeAsync(int id)
        {
            return await _context.MenuItems
                .Include(x => x.Recipes)
                .ThenInclude(x => x.Ingredient)
                .FirstOrDefaultAsync(x => x.MenuItemId == id);
        }

        // Thêm món ăn mới
        public async Task AddAsync(MenuItem menuItem)
        {
            await _context.MenuItems.AddAsync(menuItem);
        }

        // Cập nhật món ăn: tên món, giá bán
        public void Update(MenuItem menuItem)
        {
            _context.MenuItems.Update(menuItem);
        }

        // Xóa mềm món ăn để không ảnh hưởng đơn hàng cũ
        public void SoftDelete(MenuItem menuItem)
        {
            menuItem.IsActive = false;
            _context.MenuItems.Update(menuItem);
        }
    }
}
