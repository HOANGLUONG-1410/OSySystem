using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.DTOs.Adjustments;
using Services.Interfaces;

namespace Web_API.Controllers
{
    [ApiController]
    [Route("api/adjustments")]
    [Authorize(Roles = "SystemAdmin,BranchManager")]
    public class AdjustmentsController : ControllerBase
    {
        private readonly IAdjustmentService _adjustmentService;

        public AdjustmentsController(
            IAdjustmentService adjustmentService)
        {
            _adjustmentService = adjustmentService;
        }

        // Admin xem toàn bộ.
        // BranchManager chỉ nhận phiếu của Kitchen mình.
        [HttpGet]
        public async Task<IActionResult> GetAccessibleAdjustments()
        {
            var result = await _adjustmentService
                .GetAccessibleAdjustmentsAsync();

            return Ok(result);
        }

        // Lấy phiếu điều chỉnh theo Kitchen
        // Service vẫn kiểm tra KitchenId để tránh truy cập chéo
        [HttpGet("kitchen/{kitchenId:int}")]
        public async Task<IActionResult> GetByKitchenId(
            int kitchenId)
        {
            try
            {
                var result = await _adjustmentService
                    .GetByKitchenIdAsync(kitchenId);

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new
                {
                    message = ex.Message
                });
            }
        }

        // Tạo phiếu điều chỉnh tồn kho
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateAdjustmentRequest request)
        {
            try
            {
                var result = await _adjustmentService
                    .CreateAsync(request);

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new
                {
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
    }
}
