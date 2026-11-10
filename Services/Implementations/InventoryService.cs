using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using Repositories.Interfaces;
using Services.DTOs.Inventories;
using Services.Exceptions;
using Services.Interfaces;

namespace Services.Implementations
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IInventoryLogRepository _inventoryLogRepository;
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IKitchenRepository _kitchenRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public InventoryService(
            IInventoryRepository inventoryRepository,
            IInventoryLogRepository inventoryLogRepository,
            IIngredientRepository ingredientRepository,
            IKitchenRepository kitchenRepository,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork)
        {
            _inventoryRepository = inventoryRepository;
            _inventoryLogRepository = inventoryLogRepository;
            _ingredientRepository = ingredientRepository;
            _kitchenRepository = kitchenRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Lấy danh sách tồn kho mà người dùng được phép xem.
        /// - SystemAdmin xem toàn bộ hệ thống.
        /// - Các role khác chỉ xem Kitchen mình.
        /// </summary>
        public async Task<List<InventoryResponse>> GetAccessibleInventoriesAsync()
        {
            var currentUser = _currentUserService.GetCurrentUser();

            List<Inventory> inventories;

            if (currentUser.Role == AccountRole.SystemAdmin)
            {
                inventories = await _inventoryRepository.GetAllAsync();
            }
            else
            {
                var kitchenId = RequireUserKitchen(currentUser);
                inventories = await _inventoryRepository.GetByKitchenIdAsync(kitchenId);
            }

            return inventories.Select(MapInventoryToResponse).ToList();
        }

        /// <summary>
        /// Lấy tồn kho của một Kitchen cụ thể.
        /// Kiểm tra người dùng có quyền truy cập Kitchen hay không.
        /// </summary>
        public async Task<List<InventoryResponse>> GetByKitchenIdAsync(int kitchenId)
        {
            var currentUser = _currentUserService.GetCurrentUser();

            EnsureCanAccessKitchen(currentUser, kitchenId);

            var kitchen = await _kitchenRepository.GetByIdAsync(kitchenId);

            if (kitchen == null)
            {
                throw new NotFoundException("Kitchen does not exist.");
            }

            var inventories = await _inventoryRepository.GetByKitchenIdAsync(kitchenId);

            return inventories.Select(MapInventoryToResponse).ToList();
        }

        /// <summary>
        /// Lấy tồn kho cụ thể của một nguyên liệu tại một Kitchen.
        /// </summary>
        public async Task<InventoryResponse> GetByKitchenAndIngredientAsync(int kitchenId, int ingredientId)
        {
            var currentUser = _currentUserService.GetCurrentUser();

            EnsureCanAccessKitchen(currentUser, kitchenId);

            var inventory = await _inventoryRepository.GetByKitchenAndIngredientAsync(kitchenId, ingredientId);

            if (inventory == null)
            {
                throw new NotFoundException("Inventory record does not exist.");
            }

            // Nếu DAO chưa Include Kitchen/Ingredient, lấy thêm dữ liệu để response đầy đủ
            inventory.Kitchen = inventory.Kitchen ?? await _kitchenRepository.GetByIdAsync(kitchenId);
            inventory.Ingredient = inventory.Ingredient ?? await _ingredientRepository.GetByIdAsync(ingredientId);

            return MapInventoryToResponse(inventory);
        }

        /// <summary>
        /// Khởi tạo dòng tồn kho mới.
        /// Chỉ SystemAdmin và KitchenManager được phép.
        /// Nếu InitialQuantity khác 0, ghi InventoryLog loại Import.
        /// </summary>
        public async Task<InventoryResponse> CreateInventoryAsync(CreateInventoryRequest request)
        {
            var currentUser = _currentUserService.GetCurrentUser();

            // Chỉ SystemAdmin và KitchenManager được khởi tạo kho
            if (currentUser.Role != AccountRole.SystemAdmin &&
                currentUser.Role != AccountRole.KitchenManager)
            {
                throw new ForbiddenException("You do not have permission to create inventory.");
            }

            EnsureCanAccessKitchen(currentUser, request.KitchenId);

            var kitchen = await _kitchenRepository.GetByIdAsync(request.KitchenId);

            if (kitchen == null || !kitchen.IsActive)
            {
                throw new BusinessException("Kitchen does not exist or is inactive.");
            }

            var ingredient = await _ingredientRepository.GetByIdAsync(request.IngredientId);

            if (ingredient == null || !ingredient.IsActive)
            {
                throw new BusinessException("Ingredient does not exist or is inactive.");
            }

            var existingInventory = await _inventoryRepository.GetByKitchenAndIngredientAsync(request.KitchenId, request.IngredientId);

            if (existingInventory != null)
            {
                throw new BusinessException("Inventory record already exists.");
            }

            await using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                var inventory = new Inventory
                {
                    KitchenId = request.KitchenId,
                    IngredientId = request.IngredientId,
                    Quantity = request.InitialQuantity,
                    LastUpdated = DateTime.UtcNow,
                    Kitchen = kitchen,
                    Ingredient = ingredient
                };

                await _inventoryRepository.AddAsync(inventory);

                // Nếu InitialQuantity khác 0, ghi log Import để audit
                if (request.InitialQuantity != 0)
                {
                    var log = new InventoryLog
                    {
                        KitchenId = request.KitchenId,
                        IngredientId = request.IngredientId,
                        PerformedByAccountId = currentUser.AccountId,
                        LogType = InventoryLogType.Import,
                        QuantityChange = request.InitialQuantity,
                        QuantityBefore = 0m,
                        QuantityAfter = request.InitialQuantity,
                        Note = "Initial inventory quantity.",
                        CreatedAt = DateTime.UtcNow
                    };

                    await _inventoryLogRepository.AddAsync(log);
                }

                await _unitOfWork.SaveChangesAsync();

                await transaction.CommitAsync();

                return MapInventoryToResponse(inventory);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// Nhập kho từ nhà cung cấp.
        /// Chỉ SystemAdmin và KitchenManager được phép.
        /// KitchenManager chỉ nhập vào Central Kitchen của mình.
        /// Cập nhật AverageCost bằng công thức giá vốn bình quân.
        /// </summary>
        public async Task<InventoryResponse> ImportInventoryAsync(ImportInventoryRequest request)
        {
            var currentUser = _currentUserService.GetCurrentUser();

            // Chỉ SystemAdmin và KitchenManager được nhập kho
            if (currentUser.Role != AccountRole.SystemAdmin &&
                currentUser.Role != AccountRole.KitchenManager)
            {
                throw new ForbiddenException("You do not have permission to import inventory.");
            }

            EnsureCanAccessKitchen(currentUser, request.KitchenId);

            var kitchen = await _kitchenRepository.GetByIdAsync(request.KitchenId);

            if (kitchen == null || !kitchen.IsActive)
            {
                throw new BusinessException("Kitchen does not exist or is inactive.");
            }

            var ingredient = await _ingredientRepository.GetByIdAsync(request.IngredientId);

            if (ingredient == null || !ingredient.IsActive)
            {
                throw new BusinessException("Ingredient does not exist or is inactive.");
            }

            // KitchenManager chỉ được nhập cho Central Kitchen của mình
            if (currentUser.Role == AccountRole.KitchenManager && !kitchen.IsCentralKitchen)
            {
                throw new ForbiddenException("KitchenManager can only import inventory into their central kitchen.");
            }

            await using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                var inventory = await _inventoryRepository.GetByKitchenAndIngredientAsync(request.KitchenId, request.IngredientId);

                var quantityBefore = inventory?.Quantity ?? 0m;
                var quantityAfter = quantityBefore + request.Quantity;

                // Tính giá vốn bình quân: (Tổng giá trị cũ + Tổng giá trị nhập) / Tổng số lượng mới
                var oldInventoryValue = quantityBefore * ingredient.AverageCost;
                var importedValue = request.Quantity * request.UnitCost;
                var newAverageCost = quantityAfter > 0m
                    ? (oldInventoryValue + importedValue) / quantityAfter
                    : request.UnitCost;

                if (inventory == null)
                {
                    inventory = new Inventory
                    {
                        KitchenId = request.KitchenId,
                        IngredientId = request.IngredientId,
                        Quantity = quantityAfter,
                        LastUpdated = DateTime.UtcNow
                    };

                    await _inventoryRepository.AddAsync(inventory);
                }
                else
                {
                    inventory.Quantity = quantityAfter;
                    inventory.LastUpdated = DateTime.UtcNow;

                    _inventoryRepository.Update(inventory);
                }

                // Cập nhật giá vốn nguyên liệu (toàn hệ thống)
                ingredient.AverageCost = newAverageCost;
                _ingredientRepository.Update(ingredient);

                // Ghi log Import có PerformedByAccountId để audit
                var inventoryLog = new InventoryLog
                {
                    KitchenId = request.KitchenId,
                    IngredientId = request.IngredientId,
                    PerformedByAccountId = currentUser.AccountId,
                    LogType = InventoryLogType.Import,
                    QuantityChange = request.Quantity,
                    QuantityBefore = quantityBefore,
                    QuantityAfter = quantityAfter,
                    Note = request.Note?.Trim(),
                    CreatedAt = DateTime.UtcNow
                };

                await _inventoryLogRepository.AddAsync(inventoryLog);

                await _unitOfWork.SaveChangesAsync();

                await transaction.CommitAsync();

                inventory.Kitchen = kitchen;
                inventory.Ingredient = ingredient;

                return MapInventoryToResponse(inventory);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// Lấy tất cả InventoryLog mà người dùng được phép xem.
        /// </summary>
        public async Task<List<InventoryLogResponse>> GetAccessibleLogsAsync()
        {
            var currentUser = _currentUserService.GetCurrentUser();

            List<InventoryLog> logs;

            if (currentUser.Role == AccountRole.SystemAdmin)
            {
                logs = await _inventoryLogRepository.GetAllAsync();
            }
            else
            {
                var kitchenId = RequireUserKitchen(currentUser);
                logs = await _inventoryLogRepository.GetByKitchenIdAsync(kitchenId);
            }

            return logs.Select(MapLogToResponse).ToList();
        }

        /// <summary>
        /// Lấy InventoryLog của một Kitchen cụ thể.
        /// </summary>
        public async Task<List<InventoryLogResponse>> GetLogsByKitchenIdAsync(int kitchenId)
        {
            var currentUser = _currentUserService.GetCurrentUser();

            EnsureCanAccessKitchen(currentUser, kitchenId);

            var logs = await _inventoryLogRepository.GetByKitchenIdAsync(kitchenId);

            return logs.Select(MapLogToResponse).ToList();
        }

        /// <summary>
        /// Kiểm tra người dùng có KitchenId hay không.
        /// Được dùng cho các role không phải SystemAdmin.
        /// </summary>
        private static int RequireUserKitchen(CurrentUserInfo currentUser)
        {
            if (!currentUser.KitchenId.HasValue)
            {
                throw new ForbiddenException("The current account is not assigned to a kitchen.");
            }

            return currentUser.KitchenId.Value;
        }

        /// <summary>
        /// Kiểm tra người dùng có quyền truy cập Kitchen hay không.
        /// - SystemAdmin được truy cập tất cả Kitchen.
        /// - Các role khác chỉ được truy cập Kitchen của mình.
        /// </summary>
        private static void EnsureCanAccessKitchen(CurrentUserInfo currentUser, int targetKitchenId)
        {
            if (currentUser.Role == AccountRole.SystemAdmin)
            {
                return;
            }

            var currentKitchenId = RequireUserKitchen(currentUser);

            if (currentKitchenId != targetKitchenId)
            {
                throw new ForbiddenException("You cannot access another kitchen's inventory.");
            }
        }

        private static InventoryResponse MapInventoryToResponse(Inventory inventory)
        {
            var averageCost = inventory.Ingredient?.AverageCost ?? 0m;

            return new InventoryResponse
            {
                InventoryId = inventory.InventoryId,
                KitchenId = inventory.KitchenId,
                KitchenName = inventory.Kitchen?.KitchenName ?? string.Empty,
                IngredientId = inventory.IngredientId,
                IngredientName = inventory.Ingredient?.IngredientName ?? string.Empty,
                Unit = inventory.Ingredient?.Unit ?? string.Empty,
                Quantity = inventory.Quantity,
                AverageCost = averageCost,
                InventoryValue = inventory.Quantity * averageCost,
                LastUpdated = inventory.LastUpdated,
                IsNegative = inventory.Quantity < 0
            };
        }

        private static InventoryLogResponse MapLogToResponse(InventoryLog log)
        {
            return new InventoryLogResponse
            {
                InventoryLogId = log.InventoryLogId,
                KitchenId = log.KitchenId,
                KitchenName = log.Kitchen?.KitchenName ?? string.Empty,
                IngredientId = log.IngredientId,
                IngredientName = log.Ingredient?.IngredientName ?? string.Empty,
                PerformedByAccountId = log.PerformedByAccountId,
                PerformedByName = log.PerformedByAccount?.FullName ?? "System",
                LogType = log.LogType.ToString(),
                QuantityChange = log.QuantityChange,
                QuantityBefore = log.QuantityBefore,
                QuantityAfter = log.QuantityAfter,
                Note = log.Note,
                CreatedAt = log.CreatedAt,
            };
        }
    }
}
