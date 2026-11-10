using System.Security.Claims;
using BusinessObjects.Enums;
using Services.Exceptions;
using Services.Interfaces;

namespace Web_API.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(
            IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public CurrentUserInfo GetCurrentUser()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            // Không có HttpContext hoặc người dùng chưa đăng nhập
            if (user?.Identity?.IsAuthenticated != true)
            {
                throw new AuthenticationException(
                    "User is not authenticated.");
            }

            var accountIdValue = user.FindFirst("AccountId")?.Value;
            var username = user.FindFirst(ClaimTypes.Name)?.Value;
            var roleValue = user.FindFirst(ClaimTypes.Role)?.Value;
            var kitchenIdValue = user.FindFirst("KitchenId")?.Value;

            if (!int.TryParse(accountIdValue, out var accountId))
            {
                throw new AuthenticationException(
                    "AccountId claim is missing or invalid.");
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                throw new AuthenticationException(
                    "Username claim is missing.");
            }

            if (!Enum.TryParse<AccountRole>(
                    roleValue,
                    out var role))
            {
                throw new AuthenticationException(
                    "Role claim is missing or invalid.");
            }

            int? kitchenId = null;

            if (int.TryParse(kitchenIdValue, out var parsedKitchenId))
            {
                kitchenId = parsedKitchenId;
            }

            return new CurrentUserInfo
            {
                AccountId = accountId,
                Username = username,
                Role = role,
                KitchenId = kitchenId
            };
        }
    }
}
