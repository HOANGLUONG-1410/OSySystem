using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.DTOs.Recipes;
using Services.Exceptions;
using Services.Interfaces;

namespace Web_API.Controllers
{
    [ApiController]
    [Route("api/recipes")]
    [Authorize]
    public class RecipesController : ControllerBase
    {
        private readonly IRecipeService _recipeService;

        public RecipesController(IRecipeService recipeService)
        {
            _recipeService = recipeService;
        }

        // Admin và KitchenManager xem toàn bộ công thức
        [Authorize(Roles = "SystemAdmin,KitchenManager")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _recipeService.GetAllAsync();

            return Ok(result);
        }

        // Lấy công thức theo món ăn
        // Có thể dùng khi hiển thị chi tiết món
        [Authorize(Roles = "SystemAdmin,KitchenManager")]
        [HttpGet("menu-item/{menuItemId:int}")]
        public async Task<IActionResult> GetByMenuItemId(int menuItemId)
        {
            try
            {
                var result = await _recipeService
                    .GetByMenuItemIdAsync(menuItemId);

                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "SystemAdmin,KitchenManager")]
        [HttpGet("{recipeId:int}")]
        public async Task<IActionResult> GetById(int recipeId)
        {
            try
            {
                var result = await _recipeService
                    .GetByIdAsync(recipeId);

                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // Admin và KitchenManager được cấu hình công thức
        [Authorize(Roles = "SystemAdmin,KitchenManager")]
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateRecipeRequest request)
        {
            try
            {
                var result = await _recipeService.CreateAsync(request);

                return CreatedAtAction(
                    nameof(GetById),
                    new { recipeId = result.RecipeId },
                    result);
            }
            catch (BusinessException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "SystemAdmin,KitchenManager")]
        [HttpPut("{recipeId:int}")]
        public async Task<IActionResult> Update(
            int recipeId,
            [FromBody] UpdateRecipeRequest request)
        {
            try
            {
                var result = await _recipeService.UpdateAsync(
                    recipeId,
                    request);

                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "SystemAdmin,KitchenManager")]
        [HttpDelete("{recipeId:int}")]
        public async Task<IActionResult> Delete(int recipeId)
        {
            try
            {
                await _recipeService.DeleteAsync(recipeId);

                return Ok(new
                {
                    message = "Recipe has been deleted."
                });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
