using BusinessObjects.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.DTOs.Auth;
using Services.DTOs.Common;
using Services.Exceptions;
using Services.Interfaces;
using Web_API.Validators;

namespace Web_API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ICurrentUserService _currentUserService;
        private readonly AuthValidator _authValidator;

        public AuthController(
            IAuthService authService,
            ICurrentUserService currentUserService,
            AuthValidator authValidator)
        {
            _authService = authService;
            _currentUserService = currentUserService;
            _authValidator = authValidator;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var validation = _authValidator.ValidateLogin(request);
            if (!validation.IsValid)
                return BadRequest(new ApiMessageResponse(validation.Message));

            try
            {
                var result = await _authService.LoginAsync(request);
                return Ok(result);
            }
            catch (AuthenticationException ex)
            {
                return Unauthorized(new ApiMessageResponse(ex.Message));
            }
        }

        [Authorize(Roles = "SystemAdmin,BranchManager")]
        [HttpPost("accounts")]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequest request)
        {
            var validation = _authValidator.ValidateCreateAccount(request);
            if (!validation.IsValid)
                return BadRequest(new ApiMessageResponse(validation.Message));

            try
            {
                var currentUser = _currentUserService.GetCurrentUser();

                if (currentUser.Role == AccountRole.BranchManager)
                {
                    if (request.Role != AccountRole.Staff)
                        return BadRequest(new ApiMessageResponse("Quản lý chi nhánh chỉ được tạo tài khoản nhân viên."));

                    if (request.KitchenId != currentUser.KitchenId)
                        return BadRequest(new ApiMessageResponse("Quản lý chi nhánh chỉ được tạo tài khoản trong chi nhánh của mình."));
                }

                var result = await _authService.CreateAccountAsync(request);
                return CreatedAtAction(nameof(CreateAccount), new { id = result.AccountId }, result);
            }
            catch (BusinessException ex)
            {
                return BadRequest(new ApiMessageResponse(ex.Message));
            }
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult GetCurrentUser()
        {
            return Ok(new CurrentUserResponse
            {
                AccountId = User.FindFirst("AccountId")?.Value,
                Username = User.Identity?.Name,
                Role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value,
                KitchenId = User.FindFirst("KitchenId")?.Value
            });
        }
    }
}
