using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using Repositories.Interfaces;
using Services.DTOs.Accounts;
using Services.DTOs.Auth;
using Services.Exceptions;
using Services.Interfaces;

namespace Services.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly ISystemAccountRepository _accountRepository;
        private readonly IKitchenRepository _kitchenRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AccountService(
            ISystemAccountRepository accountRepository,
            IKitchenRepository kitchenRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _accountRepository = accountRepository;
            _kitchenRepository = kitchenRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Lấy danh sách tất cả tài khoản.
        /// Chỉ SystemAdmin được gọi (kiểm tra ở Controller).
        /// </summary>
        public async Task<List<AccountResponse>> GetAllAsync()
        {
            // Repository lấy toàn bộ tài khoản kèm Kitchen
            var accounts = await _accountRepository.GetAllAsync();

            // Chuyển Entity sang DTO, không trả PasswordHash
            return _mapper.Map<List<AccountResponse>>(accounts);
        }

        /// <summary>
        /// Lấy danh sách tài khoản theo Kitchen.
        /// Dành cho BranchManager xem nhân sự trong chi nhánh.
        /// </summary>
        public async Task<List<AccountResponse>> GetByKitchenIdAsync(int kitchenId)
        {
            var accounts = await _accountRepository.GetByKitchenIdAsync(kitchenId);

            return _mapper.Map<List<AccountResponse>>(accounts);
        }

        /// <summary>
        /// Lấy thông tin một tài khoản theo ID.
        /// </summary>
        public async Task<AccountResponse> GetByIdAsync(int accountId)
        {
            // Tìm tài khoản theo AccountId
            var account = await _accountRepository.GetByIdAsync(accountId);

            if (account == null)
            {
                throw new NotFoundException(
                    "Account does not exist.");
            }

            return _mapper.Map<AccountResponse>(account);
        }

        /// <summary>
        /// Cập nhật thông tin tài khoản (FullName, Role, Kitchen).
        /// Chỉ SystemAdmin được cập nhật (kiểm tra ở Controller).
        /// </summary>
        public async Task<AccountResponse> UpdateAsync(
            int accountId,
            UpdateAccountRequest request)
        {
            // Tìm tài khoản cần cập nhật
            var account = await _accountRepository.GetByIdAsync(accountId);

            if (account == null)
            {
                throw new NotFoundException(
                    "Account does not exist.");
            }

            // Kiểm tra logic Role và Kitchen trước khi cập nhật
            await ValidateRoleAndKitchenAsync(
                request.Role,
                request.KitchenId);

            account.FullName = request.FullName.Trim();
            account.Role = request.Role;
            account.KitchenId = request.KitchenId;

            // Repository đánh dấu entity đã thay đổi
            _accountRepository.Update(account);

            // Lưu thay đổi xuống database
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<AccountResponse>(account);
        }

        /// <summary>
        /// Khóa tài khoản (xóa mềm).
        /// Không được khóa SystemAdmin cuối cùng.
        /// </summary>
        public async Task DeactivateAsync(int accountId)
        {
            var account = await _accountRepository.GetByIdAsync(accountId);

            if (account == null)
            {
                throw new NotFoundException(
                    "Account does not exist.");
            }

            // Không nên khóa tài khoản SystemAdmin cuối cùng
            if (account.Role == AccountRole.SystemAdmin)
            {
                var admins = await _accountRepository
                    .GetByRoleAsync(AccountRole.SystemAdmin);

                var activeAdminCount = admins.Count(x => x.IsActive);

                if (activeAdminCount <= 1)
                {
                    throw new BusinessException(
                        "The last active SystemAdmin cannot be deactivated.");
                }
            }

            // Xóa mềm bằng cách đổi IsActive thành false
            _accountRepository.Deactivate(account);

            await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Mở khóa tài khoản.
        /// </summary>
        public async Task ActivateAsync(int accountId)
        {
            var account = await _accountRepository.GetByIdAsync(accountId);

            if (account == null)
            {
                throw new NotFoundException(
                    "Account does not exist.");
            }

            // Nếu tài khoản đã hoạt động thì không cần cập nhật lại
            if (account.IsActive)
            {
                return;
            }

            _accountRepository.Activate(account);

            await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Người dùng tự đổi mật khẩu của mình.
        /// Cần xác minh mật khẩu cũ.
        /// </summary>
        public async Task ChangePasswordAsync(
            int accountId,
            ChangePasswordRequest request)
        {
            var account = await _accountRepository.GetByIdAsync(accountId);

            if (account == null)
            {
                throw new NotFoundException(
                    "Account does not exist.");
            }

            // Xác minh mật khẩu hiện tại
            var currentPasswordValid = BCrypt.Net.BCrypt.Verify(
                request.CurrentPassword,
                account.PasswordHash);

            if (!currentPasswordValid)
            {
                throw new AuthenticationException(
                    "Current password is incorrect.");
            }

            // Không cho đổi lại đúng mật khẩu cũ
            var sameAsOldPassword = BCrypt.Net.BCrypt.Verify(
                request.NewPassword,
                account.PasswordHash);

            if (sameAsOldPassword)
            {
                throw new BusinessException(
                    "New password must be different from current password.");
            }

            // Hash mật khẩu mới trước khi lưu
            account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(
                request.NewPassword);

            _accountRepository.Update(account);

            await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Admin reset mật khẩu cho người dùng khác.
        /// Không cần xác minh mật khẩu cũ.
        /// </summary>
        public async Task ResetPasswordAsync(
            int accountId,
            ResetPasswordRequest request)
        {
            var account = await _accountRepository.GetByIdAsync(accountId);

            if (account == null)
            {
                throw new NotFoundException(
                    "Account does not exist.");
            }

            // Admin không cần biết mật khẩu cũ
            account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(
                request.NewPassword);

            _accountRepository.Update(account);

            await _unitOfWork.SaveChangesAsync();
        }

        private async Task ValidateRoleAndKitchenAsync(
            AccountRole role,
            int? kitchenId)
        {
            // SystemAdmin quản lý toàn hệ thống
            if (role == AccountRole.SystemAdmin)
            {
                return;
            }

            // Các role vận hành bắt buộc thuộc một Kitchen
            if (!kitchenId.HasValue)
            {
                throw new BusinessException(
                    $"{role} must belong to a kitchen.");
            }

            // Kiểm tra Kitchen có tồn tại hay không
            var kitchen = await _kitchenRepository
                .GetByIdAsync(kitchenId.Value);

            if (kitchen == null || !kitchen.IsActive)
            {
                throw new BusinessException(
                    "Kitchen does not exist or is inactive.");
            }

            // KitchenManager chỉ nên thuộc Bếp tổng
            if (role == AccountRole.KitchenManager &&
                !kitchen.IsCentralKitchen)
            {
                throw new BusinessException(
                    "KitchenManager must belong to the central kitchen.");
            }

            // BranchManager và Staff chỉ nên thuộc chi nhánh
            if ((role == AccountRole.BranchManager ||
                 role == AccountRole.Staff) &&
                kitchen.IsCentralKitchen)
            {
                throw new BusinessException(
                    $"{role} cannot belong to the central kitchen.");
            }
        }
    }
}
