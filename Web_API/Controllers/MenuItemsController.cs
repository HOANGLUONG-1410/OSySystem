using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.DTOs.MenuItem;
using Services.Exceptions;
using Services.Interfaces;

namespace Web_API.Controllers
{
    [ApiController]
    [Route("api/menu-items")]
    [Authorize]
    public class MenuItemsController : ControllerBase
    {
        private readonly IMenuItemService _menuItemService;

        public MenuItemsController(
            IMenuItemService menuItemService)
        {
            _menuItemService = menuItemService;
        }

        // Tất cả tài khoản đăng nhập được xem menu
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _menuItemService.GetAllAsync();

            return Ok(result);
        }

        [HttpGet("{menuItemId:int}")]
        public async Task<IActionResult> GetById(int menuItemId)
        {
            try
            {
                var result = await _menuItemService
                    .GetByIdAsync(menuItemId);

                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // Chỉ Admin quản lý danh mục món ăn và giá bán
        [Authorize(Roles = "SystemAdmin")]
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateMenuItemRequest request)
        {
            try
            {
                var result = await _menuItemService.CreateAsync(
                    request);

                return CreatedAtAction(
                    nameof(GetById),
                    new { menuItemId = result.MenuItemId },
                    result);
            }
            catch (BusinessException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "SystemAdmin")]
        [HttpPut("{menuItemId:int}")]
        public async Task<IActionResult> Update(
            int menuItemId,
            [FromBody] UpdateMenuItemRequest request)
        {
            try
            {
                var result = await _menuItemService.UpdateAsync(
                    menuItemId,
                    request);

                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (BusinessException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "SystemAdmin")]
        [HttpPatch("{menuItemId:int}/deactivate")]
        public async Task<IActionResult> Deactivate(int menuItemId)
        {
            try
            {
                await _menuItemService
                    .DeactivateAsync(menuItemId);

                return Ok(new
                {
                    message = "Menu item has been deactivated."
                });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
