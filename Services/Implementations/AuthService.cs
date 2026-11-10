using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Repositories.Interfaces;
using Services.DTOs.Auth;
using Services.Exceptions;
using Services.Interfaces;
using Services.SettingsJWT;


namespace Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly ISystemAccountRepository _accountRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly JwtSettings _jwtSettings;

        public AuthService(
            ISystemAccountRepository accountRepository,
            IUnitOfWork unitOfWork,
            IOptions<JwtSettings> jwtOptions)
        {
            _accountRepository = accountRepository;
            _unitOfWork = unitOfWork;
            _jwtSettings = jwtOptions.Value;
        }

        /// <summary>
        /// Đăng nhập: kiểm tra username, mật khẩu và sinh JWT.
        /// Không nói rõ username hay password sai để hạn chế lộ thông tin.
        /// </summary>
        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            // Chuẩn hóa username để tránh khoảng trắng thừa
            var normalizedUsername = request.Username.Trim();

            // Tìm tài khoản theo username
            var account = await _accountRepository
                .GetByUsernameAsync(normalizedUsername);

            if (account == null)
                throw new AuthenticationException("Tên đăng nhập hoặc mật khẩu không đúng.");

            if (!account.IsActive)
                throw new AuthenticationException("Tài khoản đã bị vô hiệu hóa.");

            var passwordValid = BCrypt.Net.BCrypt.Verify(
                request.Password,
                account.PasswordHash);

            if (!passwordValid)
                throw new AuthenticationException("Tên đăng nhập hoặc mật khẩu không đúng.");

            // Sinh JWT sau khi tài khoản và mật khẩu hợp lệ
            var expiresAt = DateTime.UtcNow
                .AddMinutes(_jwtSettings.ExpirationMinutes);

            var token = GenerateJwtToken(account, expiresAt);

            return new LoginResponse
            {
                AccessToken = token,
                ExpiresAt = expiresAt,
                AccountId = account.AccountId,
                Username = account.Username,
                FullName = account.FullName,
                Role = account.Role.ToString(),
                KitchenId = account.KitchenId,
                KitchenName = account.Kitchen?.KitchenName
            };
        }

        /// <summary>
        /// Tạo tài khoản mới.
        /// Chỉ SystemAdmin được gọi endpoint này (kiểm tra ở Controller).
        /// </summary>
        public async Task<AccountResponse> CreateAccountAsync(
            CreateAccountRequest request)
        {
            var normalizedUsername = request.Username.Trim();

            if (string.IsNullOrWhiteSpace(normalizedUsername))
                throw new BusinessException("Tên đăng nhập không được để trống.");

            if (normalizedUsername.Length > 50)
                throw new BusinessException("Tên đăng nhập không được quá 50 ký tự.");

            if (string.IsNullOrWhiteSpace(request.FullName))
                throw new BusinessException("Họ tên không được để trống.");

            if (string.IsNullOrWhiteSpace(request.Password))
                throw new BusinessException("Mật khẩu không được để trống.");

            if (request.Password.Length < 6)
                throw new BusinessException("Mật khẩu phải có ít nhất 6 ký tự.");

            if (!request.Password.Any(char.IsUpper))
                throw new BusinessException("Mật khẩu phải chứa ít nhất 1 chữ hoa (A-Z).");

            if (!request.Password.Any(char.IsLower))
                throw new BusinessException("Mật khẩu phải chứa ít nhất 1 chữ thường (a-z).");

            if (!request.Password.Any(char.IsDigit))
                throw new BusinessException("Mật khẩu phải chứa ít nhất 1 chữ số (0-9).");

            // Không cho tạo username trùng
            var usernameExists = await _accountRepository
                .IsUsernameExistsAsync(normalizedUsername);

            if (usernameExists)
            {
                throw new BusinessException("Tên đăng nhập đã tồn tại.");
            }

            ValidateAccountKitchen(request.Role, request.KitchenId);

            // Hash mật khẩu trước khi lưu database
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(
                request.Password);

            var account = new SystemAccount
            {
                Username = normalizedUsername,
                PasswordHash = passwordHash,
                FullName = request.FullName.Trim(),
                Role = request.Role,
                KitchenId = request.KitchenId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            // Repository thêm entity vào Change Tracker
            await _accountRepository.AddAsync(account);

            // UnitOfWork lưu thay đổi xuống database
            await _unitOfWork.SaveChangesAsync();

            return MapToResponse(account);
        }

        private string GenerateJwtToken(
            SystemAccount account,
            DateTime expiresAt)
        {
            // Claims là các thông tin được đóng gói trong token
            var claims = new List<Claim>
            {
                // Claim chuẩn xác định chủ sở hữu token
                new(
                    JwtRegisteredClaimNames.Sub,
                    account.AccountId.ToString()),

                // ID duy nhất của token
                new(
                    JwtRegisteredClaimNames.Jti,
                    Guid.NewGuid().ToString()),

                // Claim riêng để Controller/Service đọc AccountId thuận tiện
                new(
                    "AccountId",
                    account.AccountId.ToString()),

                new(
                    ClaimTypes.Name,
                    account.Username),

                new(
                    "FullName",
                    account.FullName),

                // ClaimTypes.Role bắt buộc để [Authorize(Roles = "...")] hoạt động
                new(
                    ClaimTypes.Role,
                    account.Role.ToString())
            };

            // SystemAdmin có thể không có KitchenId
            // Chỉ thêm claim nếu tài khoản thực sự thuộc một Kitchen
            if (account.KitchenId.HasValue)
            {
                claims.Add(new Claim(
                    "KitchenId",
                    account.KitchenId.Value.ToString()));
            }

            var keyBytes = Encoding.UTF8.GetBytes(
                _jwtSettings.SecretKey);

            var securityKey = new SymmetricSecurityKey(keyBytes);

            var credentials = new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expiresAt,
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler()
                .WriteToken(tokenDescriptor);
        }

        private static void ValidateAccountKitchen(
            AccountRole role,
            int? kitchenId)
        {
            // SystemAdmin quản lý toàn hệ thống nên không bắt buộc thuộc Kitchen
            if (role == AccountRole.SystemAdmin)
            {
                return;
            }

            // Các role vận hành phải được gắn với một Kitchen cụ thể
            if (!kitchenId.HasValue)
            {
                var roleName = role switch
                {
                    AccountRole.KitchenManager => "Quản lý bếp",
                    AccountRole.BranchManager => "Quản lý chi nhánh",
                    AccountRole.Staff => "Nhân viên",
                    _ => role.ToString()
                };
                throw new BusinessException(
                    $"{roleName} phải thuộc một chi nhánh.");
            }
        }

        private static AccountResponse MapToResponse(
            SystemAccount account)
        {
            return new AccountResponse
            {
                AccountId = account.AccountId,
                Username = account.Username,
                FullName = account.FullName,
                Role = account.Role.ToString(),
                KitchenId = account.KitchenId,
                KitchenName = account.Kitchen?.KitchenName,
                IsActive = account.IsActive,
                CreatedAt = account.CreatedAt
            };
        }
    }
}
