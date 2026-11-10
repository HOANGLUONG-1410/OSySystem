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
    public class IngredientRepository : IIngredientRepository
    {
        private readonly IngredientDAO _ingredientDAO;

        public IngredientRepository(IngredientDAO ingredientDAO)
        {
            _ingredientDAO = ingredientDAO;
        }

        // Lấy danh sách nguyên liệu để hiển thị trên màn hình quản lý
        public Task<List<Ingredient>> GetAllAsync()
        {
            return _ingredientDAO.GetAllAsync();
        }

        // Lấy một nguyên liệu cụ thể
        // Dùng khi sửa thông tin hoặc tạo Recipe
        public Task<Ingredient?> GetByIdAsync(int ingredientId)
        {
            return _ingredientDAO.GetByIdAsync(ingredientId);
        }

        // Thêm nguyên liệu mới
        public Task AddAsync(Ingredient ingredient)
        {
            return _ingredientDAO.AddAsync(ingredient);
        }

        // Cập nhật tên, đơn vị tính hoặc giá vốn
        public void Update(Ingredient ingredient)
        {
            _ingredientDAO.Update(ingredient);
        }

        // Không xóa vật lý để tránh ảnh hưởng dữ liệu kho cũ
        public void SoftDelete(Ingredient ingredient)
        {
            _ingredientDAO.SoftDelete(ingredient);
        }
    }
}
