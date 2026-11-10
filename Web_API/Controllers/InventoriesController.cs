using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.DTOs.Inventories;
using Services.Interfaces;

namespace Web_API.Controllers
{
    [ApiController]
    [Route("api/inventories")]
    [Authorize]
    public class InventoriesController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoriesController(
            IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        // Admin xem tất cả kho.
        // Các role khác chỉ nhận dữ liệu Kitchen của mình.
        [HttpGet]
        public async Task<IActionResult> GetAccessibleInventories()
        {
            var result = await _inventoryService
                .GetAccessibleInventoriesAsync();

            return Ok(result);
        }

        // Lấy toàn bộ kho của một Kitchen
        // Service tiếp tục kiểm tra KitchenId trong JWT
        [HttpGet("kitchen/{kitchenId:int}")]
        public async Task<IActionResult> GetByKitchenId(int kitchenId)
        {
            try
            {
                var result = await _inventoryService
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
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }

        // Xem tồn kho của một nguyên liệu tại một Kitchen
        [HttpGet(
            "kitchen/{kitchenId:int}/ingredient/{ingredientId:int}")]
        public async Task<IActionResult>
            GetByKitchenAndIngredient(
                int kitchenId,
                int ingredientId)
        {
            try
            {
                var result = await _inventoryService
                    .GetByKitchenAndIngredientAsync(
                        kitchenId,
                        ingredientId);

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new
                {
                    message = ex.Message
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }

        // Admin hoặc KitchenManager khởi tạo dòng tồn kho
        [Authorize(Roles = "SystemAdmin,KitchenManager")]
        [HttpPost]
        public async Task<IActionResult> CreateInventory(
            [FromBody] CreateInventoryRequest request)
        {
            try
            {
                var result = await _inventoryService
                    .CreateInventoryAsync(request);

                return CreatedAtAction(
                    nameof(GetByKitchenAndIngredient),
                    new
                    {
                        kitchenId = result.KitchenId,
                        ingredientId = result.IngredientId
                    },
                    result);
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

        // Nhập kho nguyên liệu
        // Tác vụ này cập nhật Inventory, AverageCost và InventoryLog
        [Authorize(Roles = "SystemAdmin,KitchenManager")]
        [HttpPost("import")]
        public async Task<IActionResult> ImportInventory(
            [FromBody] ImportInventoryRequest request)
        {
            try
            {
                var result = await _inventoryService
                    .ImportInventoryAsync(request);

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

        // Lấy lịch sử biến động kho theo phạm vi quyền
        [HttpGet("logs")]
        public async Task<IActionResult> GetAccessibleLogs()
        {
            var result = await _inventoryService
                .GetAccessibleLogsAsync();

            return Ok(result);
        }

        // Lấy log của một Kitchen
        [HttpGet("logs/kitchen/{kitchenId:int}")]
        public async Task<IActionResult> GetLogsByKitchenId(
            int kitchenId)
        {
            try
            {
                var result = await _inventoryService
                    .GetLogsByKitchenIdAsync(kitchenId);

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
    }
}
