using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using Repositories.Interfaces;
using Services.DTOs.Adjustments;
using Services.Exceptions;
using Services.Interfaces;

namespace Services.Implementations
{
    public class AdjustmentService : IAdjustmentService
    {
        private readonly IAdjustmentRepository _adjustmentRepository;
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IInventoryLogRepository _inventoryLogRepository;
        private readonly IKitchenRepository _kitchenRepository;
        private readonly IIngredientRepository _ingredientRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public AdjustmentService(
            IAdjustmentRepository adjustmentRepository,
            IInventoryRepository inventoryRepository,
            IInventoryLogRepository inventoryLogRepository,
            IKitchenRepository kitchenRepository,
            IIngredientRepository ingredientRepository,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork)
        {
            _adjustmentRepository = adjustmentRepository;
            _inventoryRepository = inventoryRepository;
            _inventoryLogRepository = inventoryLogRepository;
            _kitchenRepository = kitchenRepository;
            _ingredientRepository = ingredientRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Lấy danh sách phiếu điều chỉnh mà người dùng được phép xem.
        /// - SystemAdmin xem toàn bộ.
        /// - BranchManager chỉ xem Kitchen mình.
        /// </summary>
        public async Task<List<AdjustmentResponse>>
            GetAccessibleAdjustmentsAsync()
        {
            var currentUser = _currentUserService.GetCurrentUser();

            List<Adjustment> adjustments;

            if (currentUser.Role == AccountRole.SystemAdmin)
            {
                // Admin được xem toàn bộ phiếu điều chỉnh
                adjustments = await _adjustmentRepository.GetAllAsync();
            }
            else
            {
                // BranchManager chỉ xem phiếu điều chỉnh của quán mình
                var kitchenId = RequireUserKitchen(currentUser);

                adjustments = await _adjustmentRepository
                    .GetByKitchenIdAsync(kitchenId);
            }

            return adjustments
                .Select(MapToResponse)
                .ToList();
        }

        /// <summary>
        /// Lấy danh sách phiếu điều chỉnh của một Kitchen cụ thể.
        /// </summary>
        public async Task<List<AdjustmentResponse>>
            GetByKitchenIdAsync(int kitchenId)
        {
            var currentUser = _currentUserService.GetCurrentUser();

            EnsureCanAccessKitchen(currentUser, kitchenId);

            var adjustments = await _adjustmentRepository
                .GetByKitchenIdAsync(kitchenId);

            return adjustments
                .Select(MapToResponse)
                .ToList();
        }

        /// <summary>
        /// Tạo phiếu điều chỉnh kho (kiểm kê).
        /// Chỉ SystemAdmin và BranchManager được điều chỉnh (kiểm tra ở Controller).
        /// BranchManager chỉ điều chỉnh Kitchen của mình.
        /// </summary>
        public async Task<AdjustmentResponse> CreateAsync(
            CreateAdjustmentRequest request)
        {
            var currentUser = _currentUserService.GetCurrentUser();

            // Chỉ SystemAdmin hoặc BranchManager được điều chỉnh kho
            if (currentUser.Role != AccountRole.SystemAdmin &&
                currentUser.Role != AccountRole.BranchManager)
            {
                throw new ForbiddenException(
                    "You do not have permission to adjust inventory.");
            }

            EnsureCanAccessKitchen(currentUser, request.KitchenId);

            var kitchen = await _kitchenRepository
                .GetByIdAsync(request.KitchenId);

            if (kitchen == null || !kitchen.IsActive)
            {
                throw new BusinessException(
                    "Kitchen does not exist or is inactive.");
            }

            // BranchManager chỉ được quản lý chi nhánh
            if (currentUser.Role == AccountRole.BranchManager &&
                kitchen.IsCentralKitchen)
            {
                throw new ForbiddenException(
                    "BranchManager cannot adjust central kitchen inventory.");
            }

            var ingredient = await _ingredientRepository
                .GetByIdAsync(request.IngredientId);

            if (ingredient == null || !ingredient.IsActive)
            {
                throw new BusinessException(
                    "Ingredient does not exist or is inactive.");
            }

            var inventory = await _inventoryRepository
                .GetByKitchenAndIngredientAsync(
                    request.KitchenId,
                    request.IngredientId);

            if (inventory == null)
            {
                throw new BusinessException(
                    "Inventory record does not exist.");
            }

            await using var transaction =
                await _unitOfWork.BeginTransactionAsync();

            try
            {
                var systemQuantity = inventory.Quantity;

                // Chênh lệch = số lượng thực tế - số lượng hệ thống
                var differenceQuantity =
                    request.ActualQuantity - systemQuantity;

                var adjustment = new Adjustment
                {
                    KitchenId = request.KitchenId,
                    IngredientId = request.IngredientId,
                    CreatedByAccountId = currentUser.AccountId,
                    SystemQuantity = systemQuantity,
                    ActualQuantity = request.ActualQuantity,
                    DifferenceQuantity = differenceQuantity,
                    Reason = request.Reason.Trim(),
                    CreatedAt = DateTime.UtcNow,
                    Kitchen = kitchen,
                    Ingredient = ingredient
                };

                await _adjustmentRepository.AddAsync(adjustment);

                // Cập nhật tồn kho sang số lượng thực tế
                inventory.Quantity = request.ActualQuantity;
                inventory.LastUpdated = DateTime.UtcNow;

                _inventoryRepository.Update(inventory);

                // Ghi InventoryLog để audit
                var inventoryLog = new InventoryLog
                {
                    KitchenId = request.KitchenId,
                    IngredientId = request.IngredientId,
                    PerformedByAccountId = currentUser.AccountId,
                    LogType = InventoryLogType.Adjustment,
                    QuantityChange = differenceQuantity,
                    QuantityBefore = systemQuantity,
                    QuantityAfter = request.ActualQuantity,
                    Note = request.Reason.Trim(),
                    CreatedAt = DateTime.UtcNow
                };

                await _inventoryLogRepository.AddAsync(inventoryLog);

                // Nếu kho âm thì ghi warning log
                if (request.ActualQuantity < 0)
                {
                    var warningLog = new InventoryLog
                    {
                        KitchenId = request.KitchenId,
                        IngredientId = request.IngredientId,
                        PerformedByAccountId = currentUser.AccountId,
                        LogType = InventoryLogType.Warning,
                        QuantityChange = 0,
                        QuantityBefore = request.ActualQuantity,
                        QuantityAfter = request.ActualQuantity,
                        Note = "Inventory is negative after adjustment.",
                        CreatedAt = DateTime.UtcNow
                    };

                    await _inventoryLogRepository.AddAsync(warningLog);
                }

                // Lưu Adjustment, Inventory, InventoryLog cùng transaction
                await _unitOfWork.SaveChangesAsync();

                await transaction.CommitAsync();

                return MapToResponse(adjustment);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private static int RequireUserKitchen(
            CurrentUserInfo currentUser)
        {
            if (!currentUser.KitchenId.HasValue)
            {
                throw new ForbiddenException(
                    "The current account is not assigned to a kitchen.");
            }

            return currentUser.KitchenId.Value;
        }

        private static void EnsureCanAccessKitchen(
            CurrentUserInfo currentUser,
            int targetKitchenId)
        {
            if (currentUser.Role == AccountRole.SystemAdmin)
            {
                return;
            }

            var currentKitchenId = RequireUserKitchen(currentUser);

            if (currentKitchenId != targetKitchenId)
            {
                throw new ForbiddenException(
                    "You cannot adjust another kitchen's inventory.");
            }
        }

        private static AdjustmentResponse MapToResponse(
            Adjustment adjustment)
        {
            return new AdjustmentResponse
            {
                AdjustmentId = adjustment.AdjustmentId,
                KitchenId = adjustment.KitchenId,
                KitchenName =
                    adjustment.Kitchen?.KitchenName ?? string.Empty,
                IngredientId = adjustment.IngredientId,
                IngredientName =
                    adjustment.Ingredient?.IngredientName ?? string.Empty,
                CreatedByAccountId = adjustment.CreatedByAccountId,
                CreatedByName = adjustment.CreatedByAccount?.FullName ?? "Unknown",
                SystemQuantity = adjustment.SystemQuantity,
                ActualQuantity = adjustment.ActualQuantity,
                DifferenceQuantity =
                    adjustment.DifferenceQuantity,
                Reason = adjustment.Reason,
                CreatedAt = adjustment.CreatedAt
            };
        }
    }
}
