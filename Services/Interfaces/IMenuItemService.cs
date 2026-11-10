using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Services.DTOs.MenuItem;

namespace Services.Interfaces
{
    public interface IMenuItemService
    {
        Task<List<MenuItemResponse>> GetAllAsync();

        Task<MenuItemResponse> GetByIdAsync(int menuItemId);

        Task<MenuItemResponse> CreateAsync(
            CreateMenuItemRequest request);

        Task<MenuItemResponse> UpdateAsync(
            int menuItemId,
            UpdateMenuItemRequest request);

        Task DeactivateAsync(int menuItemId);
    }
}
