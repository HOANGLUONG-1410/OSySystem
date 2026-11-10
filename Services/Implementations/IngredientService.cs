using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;
using Repositories.Interfaces;
using Services.DTOs.Ingredient;
using Services.Exceptions;
using Services.Interfaces;

namespace Services.Implementations
{
    public class IngredientService : IIngredientService
    {
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IUnitOfWork _unitOfWork;

        public IngredientService(
            IIngredientRepository ingredientRepository,
            IUnitOfWork unitOfWork)
        {
            _ingredientRepository = ingredientRepository;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Lấy danh sách tất cả nguyên liệu.
        /// </summary>
        public async Task<List<IngredientResponse>> GetAllAsync()
        {
            var ingredients = await _ingredientRepository.GetAllAsync();

            return ingredients
                .Select(MapToResponse)
                .ToList();
        }

        /// <summary>
        /// Lấy thông tin một nguyên liệu theo ID.
        /// </summary>
        public async Task<IngredientResponse> GetByIdAsync(
            int ingredientId)
        {
            var ingredient = await _ingredientRepository
                .GetByIdAsync(ingredientId);

            if (ingredient == null)
            {
                throw new NotFoundException(
                    "Ingredient does not exist.");
            }

            return MapToResponse(ingredient);
        }

        /// <summary>
        /// Tạo nguyên liệu mới.
        /// Chỉ SystemAdmin và KitchenManager được tạo (kiểm tra ở Controller).
        /// </summary>
        public async Task<IngredientResponse> CreateAsync(
            CreateIngredientRequest request)
        {
            var ingredientName = request.IngredientName.Trim();
            var unit = request.Unit.Trim();

            var ingredients = await _ingredientRepository.GetAllAsync();

            // Không cho trùng tên nguyên liệu
            var duplicatedName = ingredients.Any(x =>
                x.IngredientName.Equals(
                    ingredientName,
                    StringComparison.OrdinalIgnoreCase));

            if (duplicatedName)
            {
                throw new BusinessException(
                    "Ingredient name already exists.");
            }

            var ingredient = new Ingredient
            {
                IngredientName = ingredientName,
                Unit = unit,
                AverageCost = request.AverageCost,
                IsActive = true
            };

            await _ingredientRepository.AddAsync(ingredient);

            await _unitOfWork.SaveChangesAsync();

            return MapToResponse(ingredient);
        }

        /// <summary>
        /// Cập nhật thông tin nguyên liệu.
        /// Chỉ SystemAdmin và KitchenManager được cập nhật (kiểm tra ở Controller).
        /// </summary>
        public async Task<IngredientResponse> UpdateAsync(
            int ingredientId,
            UpdateIngredientRequest request)
        {
            var ingredient = await _ingredientRepository
                .GetByIdAsync(ingredientId);

            if (ingredient == null)
            {
                throw new NotFoundException(
                    "Ingredient does not exist.");
            }

            if (!ingredient.IsActive)
            {
                throw new BusinessException(
                    "Inactive ingredient cannot be updated.");
            }

            var newName = request.IngredientName.Trim();

            var ingredients = await _ingredientRepository.GetAllAsync();

            var duplicatedName = ingredients.Any(x =>
                x.IngredientId != ingredientId &&
                x.IngredientName.Equals(
                    newName,
                    StringComparison.OrdinalIgnoreCase));

            if (duplicatedName)
            {
                throw new BusinessException(
                    "Ingredient name already exists.");
            }

            ingredient.IngredientName = newName;
            ingredient.Unit = request.Unit.Trim();
            ingredient.AverageCost = request.AverageCost;

            _ingredientRepository.Update(ingredient);

            await _unitOfWork.SaveChangesAsync();

            return MapToResponse(ingredient);
        }

        /// <summary>
        /// Khóa nguyên liệu (xóa mềm).
        /// Chỉ SystemAdmin được khóa (kiểm tra ở Controller).
        /// </summary>
        public async Task DeactivateAsync(int ingredientId)
        {
            var ingredient = await _ingredientRepository
                .GetByIdAsync(ingredientId);

            if (ingredient == null)
            {
                throw new NotFoundException(
                    "Ingredient does not exist.");
            }

            if (!ingredient.IsActive)
            {
                return;
            }

            // Không xóa vật lý để giữ Recipe và InventoryLog cũ
            _ingredientRepository.SoftDelete(ingredient);

            await _unitOfWork.SaveChangesAsync();
        }

        private static IngredientResponse MapToResponse(
            Ingredient ingredient)
        {
            return new IngredientResponse
            {
                IngredientId = ingredient.IngredientId,
                IngredientName = ingredient.IngredientName,
                Unit = ingredient.Unit,
                AverageCost = ingredient.AverageCost,
                IsActive = ingredient.IsActive
            };
        }
    }
}
