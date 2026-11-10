using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using Repositories.Interfaces;
using Services.DTOs.Orders;
using Services.Exceptions;
using Services.Interfaces;

namespace Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMenuItemRepository _menuItemRepository;
        private readonly IRecipeRepository _recipeRepository;
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IInventoryLogRepository _inventoryLogRepository;
        private readonly IKitchenRepository _kitchenRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(
            IOrderRepository orderRepository,
            IMenuItemRepository menuItemRepository,
            IRecipeRepository recipeRepository,
            IInventoryRepository inventoryRepository,
            IInventoryLogRepository inventoryLogRepository,
            IKitchenRepository kitchenRepository,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _menuItemRepository = menuItemRepository;
            _recipeRepository = recipeRepository;
            _inventoryRepository = inventoryRepository;
            _inventoryLogRepository = inventoryLogRepository;
            _kitchenRepository = kitchenRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Lấy danh sách đơn hàng mà người dùng được phép xem.
        /// - SystemAdmin xem toàn bộ hệ thống.
        /// - BranchManager, Staff chỉ xem đơn của Kitchen mình.
        /// </summary>
        public async Task<List<OrderResponse>>
            GetAccessibleOrdersAsync()
        {
            var currentUser = _currentUserService.GetCurrentUser();

            List<Order> orders;

            if (currentUser.Role == AccountRole.SystemAdmin)
            {
                // Admin xem được toàn bộ đơn hàng
                orders = await _orderRepository.GetAllAsync();
            }
            else
            {
                // Các role tại quán chỉ xem đơn thuộc Kitchen của mình
                var kitchenId = RequireUserKitchen(currentUser);

                orders = await _orderRepository
                    .GetByKitchenIdAsync(kitchenId);
            }

            return orders
                .Select(MapToResponse)
                .ToList();
        }

        /// <summary>
        /// Lấy danh sách đơn hàng theo trạng thái.
        /// </summary>
        public async Task<List<OrderResponse>> GetByStatusAsync(
            OrderStatus status)
        {
            var currentUser = _currentUserService.GetCurrentUser();

            List<Order> orders;

            if (currentUser.Role == AccountRole.SystemAdmin)
            {
                // Lấy tất cả đơn, rồi lọc theo status
                var allOrders = await _orderRepository.GetAllAsync();

                orders = allOrders
                    .Where(x => x.Status == status)
                    .ToList();
            }
            else
            {
                var kitchenId = RequireUserKitchen(currentUser);

                orders = await _orderRepository
                    .GetByKitchenAndStatusAsync(kitchenId, status);
            }

            return orders
                .Select(MapToResponse)
                .ToList();
        }

        /// <summary>
        /// Lấy chi tiết đơn hàng theo ID.
        /// Kiểm tra người dùng có quyền xem đơn này hay không.
        /// </summary>
        public async Task<OrderResponse> GetByIdAsync(int orderId)
        {
            var currentUser = _currentUserService.GetCurrentUser();

            var order = await _orderRepository.GetByIdAsync(orderId);

            if (order == null)
            {
                throw new NotFoundException(
                    "Order does not exist.");
            }

            // Ngăn người dùng của Quán A xem đơn Quán B
            EnsureCanAccessKitchen(currentUser, order.KitchenId);

            // OrderDAO đã Include Kitchen, nhưng nếu cần thêm data có thể lấy ở đây
            if (order.Kitchen == null)
            {
                order.Kitchen = await _kitchenRepository
                    .GetByIdAsync(order.KitchenId)
                    ?? throw new NotFoundException(
                        "Kitchen does not exist.");
            }

            return MapToResponse(order);
        }

        /// <summary>
        /// Tạo đơn hàng mới ở trạng thái Pending.
        /// Chưa trừ kho tại bước này.
        /// </summary>
        public async Task<OrderResponse> CreateAsync(
            CreateOrderRequest request)
        {
            var currentUser = _currentUserService.GetCurrentUser();

            // SystemAdmin, BranchManager và Staff được tạo Order
            // KitchenManager không tạo đơn bán hàng
            if (currentUser.Role != AccountRole.SystemAdmin &&
                currentUser.Role != AccountRole.BranchManager &&
                currentUser.Role != AccountRole.Staff)
            {
                throw new ForbiddenException(
                    "You do not have permission to create orders.");
            }

            EnsureCanAccessKitchen(currentUser, request.KitchenId);

            var kitchen = await _kitchenRepository
                .GetByIdAsync(request.KitchenId);

            if (kitchen == null || !kitchen.IsActive)
            {
                throw new BusinessException(
                    "Kitchen does not exist or is inactive.");
            }

            // Bếp tổng không phải điểm bán hàng
            if (kitchen.IsCentralKitchen)
            {
                throw new BusinessException(
                    "Orders cannot be created at the central kitchen.");
            }

            // Gộp các dòng trùng MenuItemId
            var groupedItems = request.Items
                .GroupBy(x => x.MenuItemId)
                .Select(group => new
                {
                    MenuItemId = group.Key,
                    Quantity = group.Sum(x => x.Quantity)
                })
                .ToList();

            var orderDetails = new List<OrderDetail>();
            decimal totalAmount = 0;

            foreach (var item in groupedItems)
            {
                var menuItem = await _menuItemRepository
                    .GetByIdAsync(item.MenuItemId);

                if (menuItem == null || !menuItem.IsActive)
                {
                    throw new BusinessException(
                        $"Menu item {item.MenuItemId} does not exist or is inactive.");
                }

                // Chốt giá bán tại thời điểm tạo đơn
                var subTotal =
                    menuItem.SellingPrice * item.Quantity;

                orderDetails.Add(new OrderDetail
                {
                    MenuItemId = menuItem.MenuItemId,
                    Quantity = item.Quantity,
                    UnitPrice = menuItem.SellingPrice,
                    SubTotal = subTotal,
                    MenuItem = menuItem
                });

                totalAmount += subTotal;
            }

            var order = new Order
            {
                KitchenId = request.KitchenId,
                CreatedByAccountId = currentUser.AccountId,
                OrderDate = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                TotalAmount = totalAmount,
                Kitchen = kitchen,
                OrderDetails = orderDetails
            };

            await _orderRepository.AddAsync(order);

            await _unitOfWork.SaveChangesAsync();

            return MapToResponse(order);
        }

        /// <summary>
        /// Xác nhận đơn hàng:
        /// - Lấy Recipe để tính nguyên liệu cần dùng.
        /// - Trừ Inventory.
        /// - Ghi InventoryLog.
        /// - Nếu kho âm, ghi Warning log.
        /// </summary>
        public async Task<ConfirmOrderResponse> ConfirmAsync(
            int orderId)
        {
            var currentUser = _currentUserService.GetCurrentUser();

            // BranchManager, Staff và Admin được confirm đơn
            if (currentUser.Role != AccountRole.SystemAdmin &&
                currentUser.Role != AccountRole.BranchManager &&
                currentUser.Role != AccountRole.Staff)
            {
                throw new ForbiddenException(
                    "You do not have permission to confirm orders.");
            }

            var order = await _orderRepository.GetByIdAsync(orderId);

            if (order == null)
            {
                throw new NotFoundException(
                    "Order does not exist.");
            }

            EnsureCanAccessKitchen(currentUser, order.KitchenId);

            // Chỉ đơn Pending mới được confirm
            if (order.Status != OrderStatus.Pending)
            {
                throw new BusinessException(
                    "Only Pending orders can be confirmed.");
            }

            if (order.OrderDetails.Count == 0)
            {
                throw new BusinessException(
                    "Order does not contain any items.");
            }

            // Tổng hợp nguyên liệu cần trừ
            var requiredIngredients =
                new Dictionary<int, decimal>();

            foreach (var orderDetail in order.OrderDetails)
            {
                var recipes = await _recipeRepository
                    .GetByMenuItemIdAsync(orderDetail.MenuItemId);

                // Không cho confirm nếu món chưa có công thức
                if (recipes.Count == 0)
                {
                    throw new BusinessException(
                        $"Menu item {orderDetail.MenuItem?.MenuItemName ?? orderDetail.MenuItemId.ToString()} does not have a recipe.");
                }

                foreach (var recipe in recipes)
                {
                    var requiredQuantity =
                        recipe.QuantityRequired *
                        orderDetail.Quantity;

                    if (requiredIngredients.ContainsKey(
                            recipe.IngredientId))
                    {
                        requiredIngredients[recipe.IngredientId] +=
                            requiredQuantity;
                    }
                    else
                    {
                        requiredIngredients[recipe.IngredientId] =
                            requiredQuantity;
                    }
                }
            }

            await using var transaction =
                await _unitOfWork.BeginTransactionAsync();

            var warnings = new List<string>();

            try
            {
                foreach (var item in requiredIngredients)
                {
                    var ingredientId = item.Key;
                    var requiredQuantity = item.Value;

                    var inventory = await _inventoryRepository
                        .GetByKitchenAndIngredientAsync(
                            order.KitchenId,
                            ingredientId);

                    decimal quantityBefore;
                    decimal quantityAfter;

                    // Nếu chưa có dòng Inventory, tạo với số lượng 0 rồi trừ
                    if (inventory == null)
                    {
                        quantityBefore = 0;
                        quantityAfter = -requiredQuantity;

                        inventory = new Inventory
                        {
                            KitchenId = order.KitchenId,
                            IngredientId = ingredientId,
                            Quantity = quantityAfter,
                            LastUpdated = DateTime.UtcNow
                        };

                        await _inventoryRepository.AddAsync(inventory);
                    }
                    else
                    {
                        quantityBefore = inventory.Quantity;
                        quantityAfter =
                            quantityBefore - requiredQuantity;

                        inventory.Quantity = quantityAfter;
                        inventory.LastUpdated = DateTime.UtcNow;

                        _inventoryRepository.Update(inventory);
                    }

                    // Ghi log trừ kho do bán hàng
                    var deductionLog = new InventoryLog
                    {
                        KitchenId = order.KitchenId,
                        IngredientId = ingredientId,
                        PerformedByAccountId = currentUser.AccountId,
                        LogType = InventoryLogType.SaleDeduction,
                        QuantityChange = -requiredQuantity,
                        QuantityBefore = quantityBefore,
                        QuantityAfter = quantityAfter,
                        Note =
                            $"Inventory deduction for Order #{order.OrderId}.",
                        CreatedAt = DateTime.UtcNow
                    };

                    await _inventoryLogRepository
                        .AddAsync(deductionLog);

                    if (quantityAfter < 0)
                    {
                        // Kho âm không chặn việc bán
                        var warningMessage =
                            $"Ingredient {ingredientId} is negative after confirming Order #{order.OrderId}. Current quantity: {quantityAfter}.";

                        warnings.Add(warningMessage);

                        var warningLog = new InventoryLog
                        {
                            KitchenId = order.KitchenId,
                            IngredientId = ingredientId,
                            PerformedByAccountId = currentUser.AccountId,
                            LogType = InventoryLogType.Warning,
                            QuantityChange = 0,
                            QuantityBefore = quantityAfter,
                            QuantityAfter = quantityAfter,
                            Note = warningMessage,
                            CreatedAt = DateTime.UtcNow
                        };

                        await _inventoryLogRepository
                            .AddAsync(warningLog);
                    }
                }

                // Chỉ đổi trạng thái sau khi đã xử lý tất cả nguyên liệu
                order.Status = OrderStatus.Confirmed;

                _orderRepository.Update(order);

                await _unitOfWork.SaveChangesAsync();

                await transaction.CommitAsync();

                return new ConfirmOrderResponse
                {
                    OrderId = order.OrderId,
                    Status = order.Status.ToString(),
                    ConfirmedAt = DateTime.UtcNow,
                    Warnings = warnings
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// Hoàn thành đơn hàng (chuyển từ Confirmed sang Completed).
        /// </summary>
        public async Task<OrderResponse> CompleteAsync(int orderId)
        {
            var currentUser = _currentUserService.GetCurrentUser();

            if (currentUser.Role != AccountRole.SystemAdmin &&
                currentUser.Role != AccountRole.BranchManager &&
                currentUser.Role != AccountRole.Staff)
            {
                throw new ForbiddenException(
                    "You do not have permission to complete orders.");
            }

            var order = await _orderRepository.GetByIdAsync(orderId);

            if (order == null)
            {
                throw new NotFoundException(
                    "Order does not exist.");
            }

            EnsureCanAccessKitchen(currentUser, order.KitchenId);

            // Chỉ đơn đã Confirmed mới được hoàn thành
            if (order.Status != OrderStatus.Confirmed)
            {
                throw new BusinessException(
                    "Only Confirmed orders can be completed.");
            }

            order.Status = OrderStatus.Completed;

            _orderRepository.Update(order);

            await _unitOfWork.SaveChangesAsync();

            order.Kitchen = await _kitchenRepository
                .GetByIdAsync(order.KitchenId);

            return MapToResponse(order);
        }

        /// <summary>
        /// Hủy đơn hàng (chỉ hủy được đơn ở trạng thái Pending).
        /// </summary>
        public async Task<OrderResponse> CancelAsync(int orderId)
        {
            var currentUser = _currentUserService.GetCurrentUser();

            if (currentUser.Role != AccountRole.SystemAdmin &&
                currentUser.Role != AccountRole.BranchManager &&
                currentUser.Role != AccountRole.Staff)
            {
                throw new ForbiddenException(
                    "You do not have permission to cancel orders.");
            }

            var order = await _orderRepository.GetByIdAsync(orderId);

            if (order == null)
            {
                throw new NotFoundException(
                    "Order does not exist.");
            }

            EnsureCanAccessKitchen(currentUser, order.KitchenId);

            // Chỉ hủy đơn Pending
            if (order.Status != OrderStatus.Pending)
            {
                throw new BusinessException(
                    "Only Pending orders can be cancelled.");
            }

            order.Status = OrderStatus.Cancelled;

            _orderRepository.Update(order);

            await _unitOfWork.SaveChangesAsync();

            order.Kitchen = await _kitchenRepository
                .GetByIdAsync(order.KitchenId);

            return MapToResponse(order);
        }

        /// <summary>
        /// Kiểm tra người dùng có KitchenId hay không.
        /// </summary>
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

        /// <summary>
        /// Kiểm tra người dùng có quyền truy cập Kitchen hay không.
        /// </summary>
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
                    "You cannot access another kitchen's orders.");
            }
        }

        private static OrderResponse MapToResponse(Order order)
        {
            return new OrderResponse
            {
                OrderId = order.OrderId,
                KitchenId = order.KitchenId,
                KitchenName =
                    order.Kitchen?.KitchenName ?? string.Empty,
                CreatedByAccountId = order.CreatedByAccountId,
                CreatedByName = order.CreatedByAccount?.FullName ?? "Unknown",
                OrderDate = order.OrderDate,
                Status = order.Status.ToString(),
                TotalAmount = order.TotalAmount,

                OrderDetails = order.OrderDetails
                    .Select(detail => new OrderDetailResponse
                    {
                        OrderDetailId = detail.OrderDetailId,
                        MenuItemId = detail.MenuItemId,
                        MenuItemName =
                            detail.MenuItem?.MenuItemName ??
                            string.Empty,
                        Quantity = detail.Quantity,
                        UnitPrice = detail.UnitPrice,
                        SubTotal = detail.SubTotal
                    })
                    .ToList()
            };
        }
    }
}
