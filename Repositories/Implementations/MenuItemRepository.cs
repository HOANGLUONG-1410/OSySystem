using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;
using DataAccessObjects.DAOs;
using Repositories.Interfaces;

namespace Repositories.Implementations
{
    public class MenuItemRepository : IMenuItemRepository
    {
        private readonly MenuItemDAO _menuItemDAO;

        public MenuItemRepository(MenuItemDAO menuItemDAO)
        {
            _menuItemDAO = menuItemDAO;
        }

        // Lấy danh sách món ăn cho màn hình bán hàng
        public Task<List<MenuItem>> GetAllAsync()
        {
            return _menuItemDAO.GetAllAsync();
        }

        // Lấy món ăn theo id để xem chi tiết hoặc sửa
        public Task<MenuItem?> GetByIdAsync(int menuItemId)
        {
            return _menuItemDAO.GetByIdAsync(menuItemId);
        }

        // Lấy món ăn kèm Recipe
        // Service sử dụng để tính nguyên liệu cần trừ
        public Task<MenuItem?> GetWithRecipeAsync(int menuItemId)
        {
            return _menuItemDAO.GetWithRecipeAsync(menuItemId);
        }

        // Thêm món ăn mới
        public Task AddAsync(MenuItem menuItem)
        {
            return _menuItemDAO.AddAsync(menuItem);
        }

        // Cập nhật tên món và giá bán
        public void Update(MenuItem menuItem)
        {
            _menuItemDAO.Update(menuItem);
        }

        // Xóa mềm để OrderDetail cũ không bị ảnh hưởng
        public void SoftDelete(MenuItem menuItem)
        {
            _menuItemDAO.SoftDelete(menuItem);
        }
    }
}
