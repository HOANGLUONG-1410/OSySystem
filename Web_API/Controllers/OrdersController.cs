using BusinessObjects.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.DTOs.Orders;
using Services.Interfaces;

namespace Web_API.Controllers
{
    [ApiController]
    [Route("api/orders")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // Admin nhận toàn bộ đơn.
        // Role tại chi nhánh chỉ nhận đơn của Kitchen mình.
        [Authorize(Roles =
            "SystemAdmin,BranchManager,Staff")]
        [HttpGet]
        public async Task<IActionResult> GetAccessibleOrders()
        {
            var result = await _orderService
                .GetAccessibleOrdersAsync();

            return Ok(result);
        }

        // Lọc đơn theo trạng thái
        // Ví dụ: /api/orders/status/Pending
        [Authorize(Roles =
            "SystemAdmin,BranchManager,Staff")]
        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetByStatus(
            OrderStatus status)
        {
            var result = await _orderService
                .GetByStatusAsync(status);

            return Ok(result);
        }

        // Lấy chi tiết một đơn
        // Service kiểm tra đơn có thuộc Kitchen người dùng không
        [Authorize(Roles =
            "SystemAdmin,BranchManager,Staff")]
        [HttpGet("{orderId:int}")]
        public async Task<IActionResult> GetById(int orderId)
        {
            try
            {
                var result = await _orderService
                    .GetByIdAsync(orderId);

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

        // Tạo đơn mới ở trạng thái Pending
        // Chưa trừ kho tại bước này
        [Authorize(Roles =
            "SystemAdmin,BranchManager,Staff")]
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateOrderRequest request)
        {
            try
            {
                var result = await _orderService.CreateAsync(request);

                return CreatedAtAction(
                    nameof(GetById),
                    new { orderId = result.OrderId },
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

        // Confirm đơn:
        // lấy Recipe, trừ Inventory, ghi InventoryLog
        [Authorize(Roles =
            "SystemAdmin,BranchManager,Staff")]
        [HttpPatch("{orderId:int}/confirm")]
        public async Task<IActionResult> Confirm(int orderId)
        {
            try
            {
                var result = await _orderService
                    .ConfirmAsync(orderId);

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
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // Chuyển đơn Confirmed sang Completed
        [Authorize(Roles =
            "SystemAdmin,BranchManager,Staff")]
        [HttpPatch("{orderId:int}/complete")]
        public async Task<IActionResult> Complete(int orderId)
        {
            try
            {
                var result = await _orderService
                    .CompleteAsync(orderId);

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
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // Hủy đơn Pending
        // Không cho hủy đơn đã Confirmed vì kho đã bị trừ
        [Authorize(Roles =
            "SystemAdmin,BranchManager,Staff")]
        [HttpPatch("{orderId:int}/cancel")]
        public async Task<IActionResult> Cancel(int orderId)
        {
            try
            {
                var result = await _orderService
                    .CancelAsync(orderId);

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
