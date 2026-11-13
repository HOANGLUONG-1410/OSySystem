using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Services.DTOs.Common;
using Services.DTOs.Recipes;
using Services.Exceptions;
using Services.Interfaces;

namespace Web_API.Controllers
{
    // [YÊU CẦU: JWT AUTHENTICATION & AUTHORIZATION] Bắt buộc đăng nhập với Bearer Token
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

        // [YÊU CẦU: ODATA + API 1: GET ALL ITEMS]
        // [EnableQuery] hỗ trợ $filter, $select, $orderby, $top, $skip trực tiếp từ URL
        [EnableQuery]
        [Authorize(Roles = "SystemAdmin,KitchenManager")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _recipeService.GetAllAsync();

            return Ok(result);
        }

        // Lấy công thức theo món ăn
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
                return NotFound(new ApiMessageResponse(ex.Message));
            }
        }

        // [YÊU CẦU: API 2: GET ITEM THEO ID]
        // Lấy thông tin chi tiết một công thức theo recipeId
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
                return NotFound(new ApiMessageResponse(ex.Message));
            }
        }

        // [YÊU CẦU: API 3: POST]
        // Tạo mới công thức món ăn từ dữ liệu gửi lên trong body
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
                return BadRequest(new ApiMessageResponse(ex.Message));
            }
        }

        // [YÊU CẦU: API 4: PUT]
        // Cập nhật toàn bộ thông tin công thức hiện có theo ID
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
                return NotFound(new ApiMessageResponse(ex.Message));
            }
        }

        // [YÊU CẦU: API 5: DELETE]
        // Xóa một công thức khỏi hệ thống theo ID
        [Authorize(Roles = "SystemAdmin,KitchenManager")]
        [HttpDelete("{recipeId:int}")]
        public async Task<IActionResult> Delete(int recipeId)
        {
            try
            {
                await _recipeService.DeleteAsync(recipeId);

                return Ok(new ApiMessageResponse("Recipe has been deleted."));
            }
            catch (NotFoundException ex)
            {
                return NotFound(new ApiMessageResponse(ex.Message));
            }
        }
    }
}
