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
    public class RecipeRepository : IRecipeRepository
    {
        private readonly RecipeDAO _recipeDAO;

        public RecipeRepository(RecipeDAO recipeDAO)
        {
            _recipeDAO = recipeDAO;
        }

        // Lấy tất cả công thức để hiển thị màn hình quản lý Recipe
        public Task<List<Recipe>> GetAllAsync()
        {
            return _recipeDAO.GetAllAsync();
        }

        // Lấy một dòng Recipe để cập nhật hoặc xóa
        public Task<Recipe?> GetByIdAsync(int recipeId)
        {
            return _recipeDAO.GetByIdAsync(recipeId);
        }

        // Lấy danh sách nguyên liệu cần cho một món ăn
        public Task<List<Recipe>> GetByMenuItemIdAsync(int menuItemId)
        {
            return _recipeDAO.GetByMenuItemIdAsync(menuItemId);
        }

        // Thêm dòng Recipe mới
        public Task AddAsync(Recipe recipe)
        {
            return _recipeDAO.AddAsync(recipe);
        }

        // Cập nhật QuantityRequired
        public void Update(Recipe recipe)
        {
            _recipeDAO.Update(recipe);
        }

        // Xóa nguyên liệu khỏi công thức món ăn
        public void Delete(Recipe recipe)
        {
            _recipeDAO.Delete(recipe);
        }
    }
}
