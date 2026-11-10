using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.DTOs.Recipes;

namespace Services.Interfaces
{
    public interface IRecipeService
    {
        Task<List<RecipeResponse>> GetAllAsync();

        Task<List<RecipeResponse>> GetByMenuItemIdAsync(
            int menuItemId);

        Task<RecipeResponse> GetByIdAsync(int recipeId);

        Task<RecipeResponse> CreateAsync(
            CreateRecipeRequest request);

        Task<RecipeResponse> UpdateAsync(
            int recipeId,
            UpdateRecipeRequest request);

        Task DeleteAsync(int recipeId);
    }
}
