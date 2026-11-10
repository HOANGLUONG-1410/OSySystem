using BusinessObjects.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.DTOs.Transfers;
using Services.Exceptions;
using Services.Interfaces;

namespace Web_API.Controllers
{
    [ApiController]
    [Route("api/transfers")]
    [Authorize(
        Roles =
            "SystemAdmin,KitchenManager,BranchManager")]
    public class TransfersController : ControllerBase
    {
        private readonly ITransferService _transferService;

        public TransfersController(
            ITransferService transferService)
        {
            _transferService = transferService;
        }

        /*
         * Admin xem toàn bộ.
         * KitchenManager xem phiếu xuất từ bếp tổng mình.
         * BranchManager xem phiếu gửi đến chi nhánh mình.
         */
        [HttpGet]
        public async Task<IActionResult> GetAccessibleTransfers()
        {
            try
            {
                var result = await _transferService
                    .GetAccessibleTransfersAsync();

                return Ok(result);
            }
            catch (ForbiddenException ex)
            {
                return StatusCode(403, new
                {
                    message = ex.Message
                });
            }
        }

        // Lọc phiếu theo trạng thái
        // Ví dụ: /api/transfers/status/InTransit
        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetByStatus(
            TransferStatus status)
        {
            try
            {
                var result = await _transferService
                    .GetByStatusAsync(status);

                return Ok(result);
            }
            catch (ForbiddenException ex)
            {
                return StatusCode(403, new
                {
                    message = ex.Message
                });
            }
        }

        // Xem chi tiết một phiếu
        [HttpGet("{transferRequestId:int}")]
        public async Task<IActionResult> GetById(
            int transferRequestId)
        {
            try
            {
                var result = await _transferService
                    .GetByIdAsync(transferRequestId);

                return Ok(result);
            }
            catch (ForbiddenException ex)
            {
                return StatusCode(403, new
                {
                    message = ex.Message
                });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }

        /*
         * Tạo phiếu Draft.
         * Chưa trừ kho ở bước này.
         */
        [Authorize(
            Roles =
                "SystemAdmin,KitchenManager")]
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateTransferRequest request)
        {
            try
            {
                var result = await _transferService
                    .CreateAsync(request);

                return CreatedAtAction(
                    nameof(GetById),
                    new
                    {
                        transferRequestId =
                            result.TransferRequestId
                    },
                    result);
            }
            catch (ForbiddenException ex)
            {
                return StatusCode(403, new
                {
                    message = ex.Message
                });
            }
            catch (BusinessException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        /*
         * Draft -> InTransit.
         * Trừ kho Bếp tổng và ghi TransferOut log.
         */
        [Authorize(
            Roles =
                "SystemAdmin,KitchenManager")]
        [HttpPatch("{transferRequestId:int}/send")]
        public async Task<IActionResult> Send(
            int transferRequestId)
        {
            try
            {
                var result = await _transferService
                    .SendAsync(transferRequestId);

                return Ok(result);
            }
            catch (ForbiddenException ex)
            {
                return StatusCode(403, new
                {
                    message = ex.Message
                });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (BusinessException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        /*
         * InTransit -> Received.
         * Cộng kho Chi nhánh và ghi TransferIn log.
         */
        [Authorize(
            Roles =
                "SystemAdmin,BranchManager")]
        [HttpPatch("{transferRequestId:int}/receive")]
        public async Task<IActionResult> Receive(
            int transferRequestId)
        {
            try
            {
                var result = await _transferService
                    .ReceiveAsync(transferRequestId);

                return Ok(result);
            }
            catch (ForbiddenException ex)
            {
                return StatusCode(403, new
                {
                    message = ex.Message
                });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (BusinessException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        /*
         * Hủy phiếu khi còn Draft.
         * Không cho hủy InTransit vì kho đã bị trừ.
         */
        [Authorize(
            Roles =
                "SystemAdmin,KitchenManager")]
        [HttpPatch("{transferRequestId:int}/cancel")]
        public async Task<IActionResult> Cancel(
            int transferRequestId)
        {
            try
            {
                var result = await _transferService
                    .CancelAsync(transferRequestId);

                return Ok(result);
            }
            catch (ForbiddenException ex)
            {
                return StatusCode(403, new
                {
                    message = ex.Message
                });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
            catch (BusinessException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
    }
}
