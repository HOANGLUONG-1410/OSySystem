using BusinessObjects.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.DTOs.Accounts;
using Services.Exceptions;
using Services.Interfaces;

namespace Web_API.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    [Authorize]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ICurrentUserService _currentUserService;

        public AccountsController(
            IAccountService accountService,
            ICurrentUserService currentUserService)
        {
            _accountService = accountService;
            _currentUserService = currentUserService;
        }

        // SystemAdmin xem toàn bộ tài khoản, BranchManager xem nhân sự trong chi nhánh
        [Authorize(Roles = "SystemAdmin,BranchManager")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var currentUser = _currentUserService.GetCurrentUser();

            if (currentUser.Role == AccountRole.BranchManager)
            {
                if (!currentUser.KitchenId.HasValue)
                    return Ok(new List<object>());

                var branchAccounts = await _accountService
                    .GetByKitchenIdAsync(currentUser.KitchenId.Value);

                return Ok(branchAccounts);
            }

            var result = await _accountService.GetAllAsync();

            return Ok(result);
        }

        // SystemAdmin xem tài khoản bất kỳ; BranchManager xem trong chi nhánh mình
        [Authorize(Roles = "SystemAdmin,BranchManager")]
        [HttpGet("{accountId:int}")]
        public async Task<IActionResult> GetById(int accountId)
        {
            try
            {
                var currentUser = _currentUserService.GetCurrentUser();
                var result = await _accountService
                    .GetByIdAsync(accountId);

                // BranchManager chỉ được xem tài khoản trong chi nhánh mình
                if (currentUser.Role == AccountRole.BranchManager)
                {
                    if (result.KitchenId != currentUser.KitchenId)
                        return Forbid();
                }

                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }

        // SystemAdmin cập nhật role hoặc Kitchen cho tài khoản
        [Authorize(Roles = "SystemAdmin")]
        [HttpPut("{accountId:int}")]
        public async Task<IActionResult> Update(
            int accountId,
            [FromBody] UpdateAccountRequest request)
        {
            try
            {
                var result = await _accountService.UpdateAsync(
                    accountId,
                    request);

                return Ok(result);
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

        // SystemAdmin khóa tài khoản
        [Authorize(Roles = "SystemAdmin")]
        [HttpPatch("{accountId:int}/deactivate")]
        public async Task<IActionResult> Deactivate(int accountId)
        {
            try
            {
                await _accountService.DeactivateAsync(accountId);

                return Ok(new
                {
                    message = "Account has been deactivated."
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

        // SystemAdmin mở khóa tài khoản
        [Authorize(Roles = "SystemAdmin")]
        [HttpPatch("{accountId:int}/activate")]
        public async Task<IActionResult> Activate(int accountId)
        {
            try
            {
                await _accountService.ActivateAsync(accountId);

                return Ok(new
                {
                    message = "Account has been activated."
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

        // Người dùng đang đăng nhập tự đổi mật khẩu của mình
        [HttpPatch("me/password")]
        public async Task<IActionResult> ChangeMyPassword(
            [FromBody] ChangePasswordRequest request)
        {
            try
            {
                // Lấy AccountId từ JWT
                var currentUser = _currentUserService.GetCurrentUser();

                await _accountService.ChangePasswordAsync(
                    currentUser.AccountId,
                    request);

                return Ok(new
                {
                    message = "Password has been changed."
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new
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

        // Chỉ Admin được reset mật khẩu cho người dùng khác
        [Authorize(Roles = "SystemAdmin")]
        [HttpPatch("{accountId:int}/reset-password")]
        public async Task<IActionResult> ResetPassword(
            int accountId,
            [FromBody] ResetPasswordRequest request)
        {
            try
            {
                await _accountService.ResetPasswordAsync(
                    accountId,
                    request);

                return Ok(new
                {
                    message = "Password has been reset."
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
    }
}
