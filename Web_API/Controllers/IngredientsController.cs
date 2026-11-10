using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.DTOs.Ingredient;
using Services.Exceptions;
using Services.Interfaces;

namespace Web_API.Controllers
{
    [ApiController]
    [Route("api/ingredients")]
    [Authorize]
    public class IngredientsController : ControllerBase
    {
        private readonly IIngredientService _ingredientService;

        public IngredientsController(
            IIngredientService ingredientService)
        {
            _ingredientService = ingredientService;
        }

        // Tất cả role đã đăng nhập được xem danh sách nguyên liệu
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _ingredientService.GetAllAsync();

            return Ok(result);
        }

        [HttpGet("{ingredientId:int}")]
        public async Task<IActionResult> GetById(int ingredientId)
        {
            try
            {
                var result = await _ingredientService
                    .GetByIdAsync(ingredientId);

                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // Admin và KitchenManager được tạo nguyên liệu
        [Authorize(Roles = "SystemAdmin,KitchenManager")]
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateIngredientRequest request)
        {
            try
            {
                var result = await _ingredientService
                    .CreateAsync(request);

                return CreatedAtAction(
                    nameof(GetById),
                    new { ingredientId = result.IngredientId },
                    result);
            }
            catch (BusinessException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Admin và KitchenManager được cập nhật nguyên liệu
        [Authorize(Roles = "SystemAdmin,KitchenManager")]
        [HttpPut("{ingredientId:int}")]
        public async Task<IActionResult> Update(
            int ingredientId,
            [FromBody] UpdateIngredientRequest request)
        {
            try
            {
                var result = await _ingredientService.UpdateAsync(
                    ingredientId,
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

        // Chỉ Admin được vô hiệu hóa nguyên liệu
        [Authorize(Roles = "SystemAdmin")]
        [HttpPatch("{ingredientId:int}/deactivate")]
        public async Task<IActionResult> Deactivate(int ingredientId)
        {
            try
            {
                await _ingredientService
                    .DeactivateAsync(ingredientId);

                return Ok(new
                {
                    message = "Ingredient has been deactivated."
                });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
