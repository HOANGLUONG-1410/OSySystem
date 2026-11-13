using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BusinessObjects.Models;
using Repositories.Interfaces;
using Services.DTOs.MenuItem;
using Services.Exceptions;
using Services.Interfaces;

namespace Services.Implementations
{
    public class MenuItemService : IMenuItemService
    {
        private readonly IMenuItemRepository _menuItemRepository;
        private readonly IRecipeRepository _recipeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MenuItemService(
            IMenuItemRepository menuItemRepository,
            IRecipeRepository recipeRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _menuItemRepository = menuItemRepository;
            _recipeRepository = recipeRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Lấy danh sách tất cả món ăn.
        /// </summary>
        public async Task<List<MenuItemResponse>> GetAllAsync()
        {
            var menuItems = await _menuItemRepository.GetAllAsync();

            return _mapper.Map<List<MenuItemResponse>>(menuItems);
        }

        /// <summary>
        /// Lấy thông tin một món ăn theo ID.
        /// </summary>
        public async Task<MenuItemResponse> GetByIdAsync(
            int menuItemId)
        {
            var menuItem = await _menuItemRepository
                .GetByIdAsync(menuItemId);

            if (menuItem == null)
            {
                throw new NotFoundException(
                    "Menu item does not exist.");
            }

            return _mapper.Map<MenuItemResponse>(menuItem);
        }

        /// <summary>
        /// Tạo món ăn mới.
        /// Chỉ SystemAdmin được tạo (kiểm tra ở Controller).
        /// </summary>
        public async Task<MenuItemResponse> CreateAsync(
            CreateMenuItemRequest request)
        {
            var menuItemName = request.MenuItemName.Trim();

            var menuItems = await _menuItemRepository.GetAllAsync();

            var duplicatedName = menuItems.Any(x =>
                x.MenuItemName.Equals(
                    menuItemName,
                    StringComparison.OrdinalIgnoreCase));

            if (duplicatedName)
            {
                throw new BusinessException(
                    "Menu item name already exists.");
            }

            var menuItem = _mapper.Map<MenuItem>(request);
            menuItem.MenuItemName = menuItemName;
            menuItem.IsActive = true;

            await _menuItemRepository.AddAsync(menuItem);

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<MenuItemResponse>(menuItem);
        }

        /// <summary>
        /// Cập nhật thông tin món ăn.
        /// Chỉ SystemAdmin được cập nhật (kiểm tra ở Controller).
        /// </summary>
        public async Task<MenuItemResponse> UpdateAsync(
            int menuItemId,
            UpdateMenuItemRequest request)
        {
            var menuItem = await _menuItemRepository
                .GetByIdAsync(menuItemId);

            if (menuItem == null)
            {
                throw new NotFoundException(
                    "Menu item does not exist.");
            }

            if (!menuItem.IsActive)
            {
                throw new BusinessException(
                    "Inactive menu item cannot be updated.");
            }

            var newName = request.MenuItemName.Trim();

            var menuItems = await _menuItemRepository.GetAllAsync();

            var duplicatedName = menuItems.Any(x =>
                x.MenuItemId != menuItemId &&
                x.MenuItemName.Equals(
                    newName,
                    StringComparison.OrdinalIgnoreCase));

            if (duplicatedName)
            {
                throw new BusinessException(
                    "Menu item name already exists.");
            }

            _mapper.Map(request, menuItem);
            menuItem.MenuItemName = newName;

            _menuItemRepository.Update(menuItem);

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<MenuItemResponse>(menuItem);
        }

        /// <summary>
        /// Khóa món ăn (xóa mềm).
        /// Chỉ SystemAdmin được khóa (kiểm tra ở Controller).
        /// </summary>
        public async Task DeactivateAsync(int menuItemId)
        {
            var menuItem = await _menuItemRepository
                .GetWithRecipeAsync(menuItemId);

            if (menuItem == null)
            {
                throw new NotFoundException(
                    "Menu item does not exist.");
            }

            if (!menuItem.IsActive)
            {
                return;
            }

            // Xoá tất cả công thức liên quan đến món ăn này
            foreach (var recipe in menuItem.Recipes)
            {
                _recipeRepository.Delete(recipe);
            }

            _menuItemRepository.SoftDelete(menuItem);

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
