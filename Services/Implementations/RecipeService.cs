using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BusinessObjects.Models;
using Repositories.Interfaces;
using Services.DTOs.Recipes;
using Services.Exceptions;
using Services.Interfaces;

namespace Services.Implementations
{
    public class RecipeService : IRecipeService
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly IMenuItemRepository _menuItemRepository;
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RecipeService(
            IRecipeRepository recipeRepository,
            IMenuItemRepository menuItemRepository,
            IIngredientRepository ingredientRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _recipeRepository = recipeRepository;
            _menuItemRepository = menuItemRepository;
            _ingredientRepository = ingredientRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Lấy danh sách tất cả công thức.
        /// </summary>
        public async Task<List<RecipeResponse>> GetAllAsync()
        {
            var recipes = await _recipeRepository.GetAllAsync();

            return _mapper.Map<List<RecipeResponse>>(recipes);
        }

        /// <summary>
        /// Lấy tất cả công thức của một món ăn.
        /// </summary>
        public async Task<List<RecipeResponse>> GetByMenuItemIdAsync(
            int menuItemId)
        {
            // Kiểm tra món ăn tồn tại trước
            var menuItem = await _menuItemRepository
                .GetByIdAsync(menuItemId);

            if (menuItem == null)
            {
                throw new NotFoundException(
                    "Menu item does not exist.");
            }

            var recipes = await _recipeRepository
                .GetByMenuItemIdAsync(menuItemId);

            return _mapper.Map<List<RecipeResponse>>(recipes);
        }

        /// <summary>
        /// Lấy thông tin một công thức theo ID.
        /// </summary>
        public async Task<RecipeResponse> GetByIdAsync(int recipeId)
        {
            var recipe = await _recipeRepository.GetByIdAsync(recipeId);

            if (recipe == null)
            {
                throw new NotFoundException(
                    "Recipe does not exist.");
            }

            // GetByIdAsync chưa Include MenuItem và Ingredient, lấy thêm dữ liệu
            var menuItem = await _menuItemRepository
                .GetByIdAsync(recipe.MenuItemId);

            var ingredient = await _ingredientRepository
                .GetByIdAsync(recipe.IngredientId);

            recipe.MenuItem = menuItem!;
            recipe.Ingredient = ingredient!;

            return _mapper.Map<RecipeResponse>(recipe);
        }

        /// <summary>
        /// Tạo công thức mới cho một món ăn.
        /// Chỉ SystemAdmin và KitchenManager được tạo (kiểm tra ở Controller).
        /// </summary>
        public async Task<RecipeResponse> CreateAsync(
            CreateRecipeRequest request)
        {
            // Kiểm tra món ăn có tồn tại và còn hoạt động không
            var menuItem = await _menuItemRepository
                .GetByIdAsync(request.MenuItemId);

            if (menuItem == null || !menuItem.IsActive)
            {
                throw new BusinessException(
                    "Menu item does not exist or is inactive.");
            }

            // Kiểm tra nguyên liệu có tồn tại và còn hoạt động không
            var ingredient = await _ingredientRepository
                .GetByIdAsync(request.IngredientId);

            if (ingredient == null || !ingredient.IsActive)
            {
                throw new BusinessException(
                    "Ingredient does not exist or is inactive.");
            }

            // Một nguyên liệu chỉ xuất hiện một lần trong công thức của một món
            var existingRecipes = await _recipeRepository
                .GetByMenuItemIdAsync(request.MenuItemId);

            var duplicatedIngredient = existingRecipes.Any(x =>
                x.IngredientId == request.IngredientId);

            if (duplicatedIngredient)
            {
                throw new BusinessException(
                    "This ingredient already exists in the recipe.");
            }

            var recipe = _mapper.Map<Recipe>(request);
            recipe.MenuItem = menuItem;
            recipe.Ingredient = ingredient;

            await _recipeRepository.AddAsync(recipe);

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<RecipeResponse>(recipe);
        }

        /// <summary>
        /// Cập nhật công thức (chỉ cập nhật định lượng).
        /// Chỉ SystemAdmin và KitchenManager được cập nhật (kiểm tra ở Controller).
        /// </summary>
        public async Task<RecipeResponse> UpdateAsync(
            int recipeId,
            UpdateRecipeRequest request)
        {
            var recipe = await _recipeRepository.GetByIdAsync(recipeId);

            if (recipe == null)
            {
                throw new NotFoundException(
                    "Recipe does not exist.");
            }

            // Chỉ cập nhật định lượng
            _mapper.Map(request, recipe);

            _recipeRepository.Update(recipe);

            await _unitOfWork.SaveChangesAsync();

            var menuItem = await _menuItemRepository
                .GetByIdAsync(recipe.MenuItemId);

            var ingredient = await _ingredientRepository
                .GetByIdAsync(recipe.IngredientId);

            recipe.MenuItem = menuItem!;
            recipe.Ingredient = ingredient!;

            return _mapper.Map<RecipeResponse>(recipe);
        }

        /// <summary>
        /// Xóa công thức vật lý (có thể xóa vì Recipe là bảng liên kết cấu hình).
        /// Chỉ SystemAdmin được xóa (kiểm tra ở Controller).
        /// </summary>
        public async Task DeleteAsync(int recipeId)
        {
            var recipe = await _recipeRepository.GetByIdAsync(recipeId);

            if (recipe == null)
            {
                throw new NotFoundException(
                    "Recipe does not exist.");
            }

            // Recipe là bảng liên kết nên có thể xóa vật lý
            _recipeRepository.Delete(recipe);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
