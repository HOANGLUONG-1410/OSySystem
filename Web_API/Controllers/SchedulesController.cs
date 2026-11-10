using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.DTOs.Schedules;
using Services.Exceptions;
using Services.Interfaces;

namespace Web_API.Controllers
{
    [ApiController]
    [Route("api/schedules")]
    [Authorize]
    public class SchedulesController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;
        private readonly ICurrentUserService _currentUserService;

        public SchedulesController(IScheduleService scheduleService, ICurrentUserService currentUserService)
        {
            _scheduleService = scheduleService;
            _currentUserService = currentUserService;
        }

        [Authorize(Roles = "BranchManager,SystemAdmin")]
        [HttpGet("kitchen/{kitchenId:int}")]
        public async Task<IActionResult> GetByKitchen(int kitchenId)
        {
            try
            {
                var result = await _scheduleService.GetByKitchenIdAsync(kitchenId);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Staff")]
        [HttpGet("my")]
        public async Task<IActionResult> GetMySchedules()
        {
            var result = await _scheduleService.GetMySchedulesAsync();
            return Ok(result);
        }

        [Authorize(Roles = "BranchManager,SystemAdmin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateScheduleRequest request)
        {
            try
            {
                var result = await _scheduleService.CreateAsync(request);
                return CreatedAtAction(nameof(GetByKitchen), new { kitchenId = result.KitchenId }, result);
            }
            catch (ForbiddenException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
            catch (BusinessException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "BranchManager,SystemAdmin")]
        [HttpPut("{scheduleId:int}")]
        public async Task<IActionResult> Update(int scheduleId, [FromBody] UpdateScheduleRequest request)
        {
            try
            {
                var result = await _scheduleService.UpdateAsync(scheduleId, request);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ForbiddenException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
            catch (BusinessException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "BranchManager,SystemAdmin")]
        [HttpDelete("{scheduleId:int}")]
        public async Task<IActionResult> Delete(int scheduleId)
        {
            try
            {
                await _scheduleService.DeleteAsync(scheduleId);
                return Ok(new { message = "Schedule deleted." });
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ForbiddenException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
        }

        [Authorize(Roles = "SystemAdmin")]
        [HttpGet("logs")]
        public async Task<IActionResult> GetLogs()
        {
            var result = await _scheduleService.GetLogsAsync();
            return Ok(result);
        }

        [Authorize(Roles = "SystemAdmin")]
        [HttpGet("logs/{logId:int}")]
        public async Task<IActionResult> GetLogDetail(int logId)
        {
            var result = await _scheduleService.GetLogDetailAsync(logId);
            if (result == null)
                return NotFound(new { message = "Log does not exist." });
            return Ok(result);
        }
    }
}
