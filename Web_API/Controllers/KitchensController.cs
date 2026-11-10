using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.DTOs.Kitchens;
using Services.Exceptions;
using Services.Interfaces;

namespace Web_API.Controllers
{
    [ApiController]
    [Route("api/kitchens")]
    [Authorize]
    public class KitchensController : ControllerBase
    {
        private readonly IKitchenService _kitchenService;

        public KitchensController(IKitchenService kitchenService)
        {
            _kitchenService = kitchenService;
        }

        // Các tài khoản đã đăng nhập được lấy danh sách Kitchen
        // Dùng cho dropdown hoặc màn hình xem chi nhánh
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _kitchenService.GetAllAsync();

            return Ok(result);
        }

        // Lấy chi tiết một Kitchen
        [HttpGet("{kitchenId:int}")]
        public async Task<IActionResult> GetById(int kitchenId)
        {
            try
            {
                var result = await _kitchenService
                    .GetByIdAsync(kitchenId);

                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // Chỉ SystemAdmin được tạo bếp hoặc chi nhánh
        [Authorize(Roles = "SystemAdmin")]
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateKitchenRequest request)
        {
            try
            {
                var result = await _kitchenService
                    .CreateAsync(request);

                return CreatedAtAction(
                    nameof(GetById),
                    new { kitchenId = result.KitchenId },
                    result);
            }
            catch (BusinessException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // Chỉ SystemAdmin được sửa Kitchen
        [Authorize(Roles = "SystemAdmin")]
        [HttpPut("{kitchenId:int}")]
        public async Task<IActionResult> Update(
            int kitchenId,
            [FromBody] UpdateKitchenRequest request)
        {
            try
            {
                var result = await _kitchenService.UpdateAsync(
                    kitchenId,
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

        // Xóa mềm chi nhánh
        [Authorize(Roles = "SystemAdmin")]
        [HttpPatch("{kitchenId:int}/deactivate")]
        public async Task<IActionResult> Deactivate(int kitchenId)
        {
            try
            {
                await _kitchenService.DeactivateAsync(kitchenId);

                return Ok(new
                {
                    message = "Kitchen has been deactivated."
                });
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
    }
}
