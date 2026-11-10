using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.DTOs.Ingredient;

namespace Services.Interfaces
{
    public interface IIngredientService
    {
        Task<List<IngredientResponse>> GetAllAsync();

        Task<IngredientResponse> GetByIdAsync(int ingredientId);

        Task<IngredientResponse> CreateAsync(
            CreateIngredientRequest request);

        Task<IngredientResponse> UpdateAsync(
            int ingredientId,
            UpdateIngredientRequest request);

        Task DeactivateAsync(int ingredientId);
    }
}
