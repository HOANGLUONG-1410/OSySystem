using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using Repositories.Interfaces;
using Services.DTOs.Transfers;
using Services.Exceptions;
using Services.Interfaces;

namespace Services.Implementations
{
    public class TransferService : ITransferService
    {
        private readonly ITransferRequestRepository
            _transferRequestRepository;

        private readonly IInventoryRepository
            _inventoryRepository;

        private readonly IInventoryLogRepository
            _inventoryLogRepository;

        private readonly IKitchenRepository
            _kitchenRepository;

        private readonly IIngredientRepository
            _ingredientRepository;

        private readonly ICurrentUserService
            _currentUserService;

        private readonly IUnitOfWork
            _unitOfWork;

        public TransferService(
            ITransferRequestRepository transferRequestRepository,
            IInventoryRepository inventoryRepository,
            IInventoryLogRepository inventoryLogRepository,
            IKitchenRepository kitchenRepository,
            IIngredientRepository ingredientRepository,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork)
        {
            _transferRequestRepository =
                transferRequestRepository;

            _inventoryRepository =
                inventoryRepository;

            _inventoryLogRepository =
                inventoryLogRepository;

            _kitchenRepository =
                kitchenRepository;

            _ingredientRepository =
                ingredientRepository;

            _currentUserService =
                currentUserService;

            _unitOfWork =
                unitOfWork;
        }

        public async Task<List<TransferRequestResponse>>
            GetAccessibleTransfersAsync()
        {
            var currentUser =
                _currentUserService.GetCurrentUser();

            List<TransferRequest> transfers;

            if (currentUser.Role == AccountRole.SystemAdmin)
            {
                // Admin xem toàn bộ phiếu điều chuyển
                transfers = await _transferRequestRepository
                    .GetAllAsync();
            }
            else if (
                currentUser.Role ==
                AccountRole.KitchenManager)
            {
                /*
                 * KitchenManager chỉ xem phiếu xuất phát từ
                 * Bếp tổng mà mình trực thuộc.
                 */
                var kitchenId =
                    RequireUserKitchen(currentUser);

                transfers = await _transferRequestRepository
                    .GetByFromKitchenIdAsync(kitchenId);
            }
            else if (
                currentUser.Role ==
                AccountRole.BranchManager)
            {
                /*
                 * BranchManager chỉ xem các phiếu được gửi đến
                 * chi nhánh của mình.
                 */
                var kitchenId =
                    RequireUserKitchen(currentUser);

                transfers = await _transferRequestRepository
                    .GetByToKitchenIdAsync(kitchenId);
            }
            else
            {
                // Staff không tham gia nghiệp vụ điều chuyển
                throw new ForbiddenException(
                    "You do not have permission to view transfers.");
            }

            return transfers
                .Select(MapToResponse)
                .ToList();
        }

        public async Task<List<TransferRequestResponse>>
            GetByStatusAsync(TransferStatus status)
        {
            var currentUser =
                _currentUserService.GetCurrentUser();

            List<TransferRequest> transfers;

            if (currentUser.Role == AccountRole.SystemAdmin)
            {
                // Admin lấy toàn bộ rồi lọc theo trạng thái
                var allTransfers =
                    await _transferRequestRepository.GetAllAsync();

                transfers = allTransfers
                    .Where(x => x.Status == status)
                    .ToList();
            }
            else if (
                currentUser.Role ==
                AccountRole.KitchenManager)
            {
                var kitchenId =
                    RequireUserKitchen(currentUser);

                var outgoingTransfers =
                    await _transferRequestRepository
                        .GetByFromKitchenIdAsync(kitchenId);

                transfers = outgoingTransfers
                    .Where(x => x.Status == status)
                    .ToList();
            }
            else if (
                currentUser.Role ==
                AccountRole.BranchManager)
            {
                var kitchenId =
                    RequireUserKitchen(currentUser);

                transfers = await _transferRequestRepository
                    .GetByToKitchenAndStatusAsync(
                        kitchenId,
                        status);
            }
            else
            {
                throw new ForbiddenException(
                    "You do not have permission to view transfers.");
            }

            return transfers
                .Select(MapToResponse)
                .ToList();
        }

        public async Task<TransferRequestResponse> GetByIdAsync(
            int transferRequestId)
        {
            var currentUser =
                _currentUserService.GetCurrentUser();

            var transfer =
                await _transferRequestRepository
                    .GetByIdAsync(transferRequestId);

            if (transfer == null)
            {
                throw new NotFoundException(
                    "Transfer request does not exist.");
            }

            // Kiểm tra người dùng hiện tại có liên quan đến phiếu này không
            EnsureCanAccessTransfer(
                currentUser,
                transfer);

            return MapToResponse(transfer);
        }

        public async Task<TransferRequestResponse> CreateAsync(
            CreateTransferRequest request)
        {
            var currentUser =
                _currentUserService.GetCurrentUser();

            /*
             * Chỉ SystemAdmin hoặc KitchenManager được tạo phiếu.
             * BranchManager chỉ tiếp nhận hàng, không được tự tạo lệnh xuất.
             */
            if (currentUser.Role != AccountRole.SystemAdmin &&
                currentUser.Role != AccountRole.KitchenManager)
            {
                throw new ForbiddenException(
                    "You do not have permission to create transfers.");
            }

            /*
             * KitchenManager chỉ được tạo phiếu từ Kitchen của mình.
             * SystemAdmin có thể chọn Bếp tổng bất kỳ.
             */
            if (currentUser.Role == AccountRole.KitchenManager)
            {
                var currentKitchenId =
                    RequireUserKitchen(currentUser);

                if (currentKitchenId != request.FromKitchenId)
                {
                    throw new ForbiddenException(
                        "You cannot create a transfer from another kitchen.");
                }
            }

            if (request.FromKitchenId == request.ToKitchenId)
            {
                throw new BusinessException(
                    "Source kitchen and destination kitchen must be different.");
            }

            var fromKitchen =
                await _kitchenRepository
                    .GetByIdAsync(request.FromKitchenId);

            var toKitchen =
                await _kitchenRepository
                    .GetByIdAsync(request.ToKitchenId);

            if (fromKitchen == null ||
                !fromKitchen.IsActive)
            {
                throw new BusinessException(
                    "Source kitchen does not exist or is inactive.");
            }

            if (toKitchen == null ||
                !toKitchen.IsActive)
            {
                throw new BusinessException(
                    "Destination kitchen does not exist or is inactive.");
            }

            // Nguồn xuất bắt buộc là Bếp tổng
            if (!fromKitchen.IsCentralKitchen)
            {
                throw new BusinessException(
                    "Transfers must be created from the central kitchen.");
            }

            // Nơi nhận phải là chi nhánh bán hàng
            if (toKitchen.IsCentralKitchen)
            {
                throw new BusinessException(
                    "Destination kitchen must be a branch.");
            }

            /*
             * Gộp các nguyên liệu trùng nhau.
             *
             * Ví dụ client gửi:
             * Thịt bò 5kg
             * Thịt bò 2kg
             *
             * Hệ thống chuyển thành Thịt bò 7kg.
             */
            var groupedItems = request.Items
                .GroupBy(x => x.IngredientId)
                .Select(group => new
                {
                    IngredientId = group.Key,
                    Quantity = group.Sum(x => x.Quantity)
                })
                .ToList();

            var transferDetails =
                new List<TransferRequestDetail>();

            foreach (var item in groupedItems)
            {
                var ingredient =
                    await _ingredientRepository
                        .GetByIdAsync(item.IngredientId);

                if (ingredient == null ||
                    !ingredient.IsActive)
                {
                    throw new BusinessException(
                        $"Ingredient {item.IngredientId} does not exist or is inactive.");
                }

                transferDetails.Add(
                    new TransferRequestDetail
                    {
                        IngredientId =
                            item.IngredientId,

                        Quantity =
                            item.Quantity,

                        Ingredient =
                            ingredient
                    });
            }

            var transferRequest =
                new TransferRequest
                {
                    FromKitchenId =
                        request.FromKitchenId,

                    ToKitchenId =
                        request.ToKitchenId,

                    Status =
                        TransferStatus.Draft,

                    CreatedByAccountId = currentUser.AccountId,

                    Note =
                        request.Note?.Trim(),

                    CreatedAt =
                        DateTime.UtcNow,

                    FromKitchen =
                        fromKitchen,

                    ToKitchen =
                        toKitchen,

                    TransferRequestDetails =
                        transferDetails
                };

            /*
             * Tạo phiếu Draft chưa thay đổi Inventory.
             * Order và Details được lưu cùng một lần SaveChanges.
             */
            await _transferRequestRepository
                .AddAsync(transferRequest);

            await _unitOfWork.SaveChangesAsync();

            return MapToResponse(transferRequest);
        }

        public async Task<TransferActionResponse> SendAsync(
            int transferRequestId)
        {
            var currentUser =
                _currentUserService.GetCurrentUser();

            /*
             * Chỉ Admin hoặc KitchenManager được xác nhận xuất hàng.
             */
            if (currentUser.Role != AccountRole.SystemAdmin &&
                currentUser.Role != AccountRole.KitchenManager)
            {
                throw new ForbiddenException(
                    "You do not have permission to send transfers.");
            }

            var transfer =
                await _transferRequestRepository
                    .GetByIdAsync(transferRequestId);

            if (transfer == null)
            {
                throw new NotFoundException(
                    "Transfer request does not exist.");
            }

            // KitchenManager chỉ xử lý phiếu của Bếp tổng mình
            if (currentUser.Role == AccountRole.KitchenManager)
            {
                var kitchenId =
                    RequireUserKitchen(currentUser);

                if (transfer.FromKitchenId != kitchenId)
                {
                    throw new ForbiddenException(
                        "You cannot send a transfer from another kitchen.");
                }
            }

            // Chỉ Draft mới được chuyển sang InTransit
            if (transfer.Status != TransferStatus.Draft)
            {
                throw new BusinessException(
                    "Only Draft transfers can be sent.");
            }

            if (transfer.TransferRequestDetails.Count == 0)
            {
                throw new BusinessException(
                    "Transfer request does not contain any items.");
            }

            await using var transaction =
                await _unitOfWork.BeginTransactionAsync();

            var warnings = new List<string>();

            try
            {
                foreach (var detail
                    in transfer.TransferRequestDetails)
                {
                    var inventory =
                        await _inventoryRepository
                            .GetByKitchenAndIngredientAsync(
                                transfer.FromKitchenId,
                                detail.IngredientId);

                    decimal quantityBefore;
                    decimal quantityAfter;

                    if (inventory == null)
                    {
                        /*
                         * Nếu Bếp tổng chưa có dòng Inventory,
                         * xem tồn ban đầu là 0 và vẫn cho xuất.
                         * Kết quả sẽ thành số âm và hệ thống ghi cảnh báo.
                         */
                        quantityBefore = 0;
                        quantityAfter = -detail.Quantity;

                        inventory = new Inventory
                        {
                            KitchenId =
                                transfer.FromKitchenId,

                            IngredientId =
                                detail.IngredientId,

                            Quantity =
                                quantityAfter,

                            LastUpdated =
                                DateTime.UtcNow
                        };

                        await _inventoryRepository
                            .AddAsync(inventory);
                    }
                    else
                    {
                        quantityBefore =
                            inventory.Quantity;

                        quantityAfter =
                            quantityBefore -
                            detail.Quantity;

                        inventory.Quantity =
                            quantityAfter;

                        inventory.LastUpdated =
                            DateTime.UtcNow;

                        _inventoryRepository.Update(
                            inventory);
                    }

                    // Ghi log xuất kho từ Bếp tổng
                    var transferOutLog =
                        new InventoryLog
                        {
                            KitchenId = transfer.FromKitchenId,
                            IngredientId = detail.IngredientId,
                            PerformedByAccountId = currentUser.AccountId,
                            LogType = InventoryLogType.TransferOut,
                            QuantityChange =- detail.Quantity,
                            QuantityBefore = quantityBefore,
                            QuantityAfter = quantityAfter,
                            Note =  $"Transfer out for Transfer Request #{transfer.TransferRequestId}.",
                            CreatedAt = DateTime.UtcNow
                        };

                    await _inventoryLogRepository
                        .AddAsync(transferOutLog);

                    if (quantityAfter < 0)
                    {
                        var warningMessage =
                            $"Ingredient {detail.IngredientId} is negative after sending Transfer Request #{transfer.TransferRequestId}. Current quantity: {quantityAfter}.";

                        warnings.Add(warningMessage);

                        // Ghi thêm Warning log để dễ audit
                        var warningLog =
                            new InventoryLog
                            {
                                KitchenId = transfer.FromKitchenId,
                                IngredientId = detail.IngredientId,
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

                // Sau khi trừ kho thành công, đổi trạng thái
                transfer.Status =
                    TransferStatus.InTransit;

                transfer.SentByAccountId = currentUser.AccountId;

                transfer.SentAt =
                    DateTime.UtcNow;

                _transferRequestRepository
                    .Update(transfer);

                /*
                 * Inventory, InventoryLog và TransferRequest
                 * được lưu trong cùng một transaction.
                 */
                await _unitOfWork.SaveChangesAsync();

                await transaction.CommitAsync();

                return new TransferActionResponse
                {
                    TransferRequestId =
                        transfer.TransferRequestId,

                    Status =
                        transfer.Status.ToString(),

                    ActionAt =
                        transfer.SentAt.Value,

                    Warnings =
                        warnings
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<TransferActionResponse> ReceiveAsync(
            int transferRequestId)
        {
            var currentUser =
                _currentUserService.GetCurrentUser();

            /*
             * Chỉ SystemAdmin hoặc BranchManager được xác nhận nhận hàng.
             */
            if (currentUser.Role != AccountRole.SystemAdmin &&
                currentUser.Role != AccountRole.BranchManager)
            {
                throw new ForbiddenException(
                    "You do not have permission to receive transfers.");
            }

            var transfer =
                await _transferRequestRepository
                    .GetByIdAsync(transferRequestId);

            if (transfer == null)
            {
                throw new NotFoundException(
                    "Transfer request does not exist.");
            }

            /*
             * BranchManager chỉ được xác nhận hàng gửi đến
             * chính chi nhánh của mình.
             */
            if (currentUser.Role == AccountRole.BranchManager)
            {
                var kitchenId =
                    RequireUserKitchen(currentUser);

                if (transfer.ToKitchenId != kitchenId)
                {
                    throw new ForbiddenException(
                        "You cannot receive a transfer for another branch.");
                }
            }

            // Chỉ phiếu đang vận chuyển mới được nhận
            if (transfer.Status != TransferStatus.InTransit)
            {
                throw new BusinessException(
                    "Only InTransit transfers can be received.");
            }

            await using var transaction =
                await _unitOfWork.BeginTransactionAsync();

            try
            {
                foreach (var detail
                    in transfer.TransferRequestDetails)
                {
                    var inventory =
                        await _inventoryRepository
                            .GetByKitchenAndIngredientAsync(
                                transfer.ToKitchenId,
                                detail.IngredientId);

                    decimal quantityBefore;
                    decimal quantityAfter;

                    if (inventory == null)
                    {
                        /*
                         * Nếu nguyên liệu chưa tồn tại trong kho chi nhánh,
                         * tạo mới Inventory với số lượng nhận được.
                         */
                        quantityBefore = 0;
                        quantityAfter =
                            detail.Quantity;

                        inventory = new Inventory
                        {
                            KitchenId =
                                transfer.ToKitchenId,

                            IngredientId =
                                detail.IngredientId,

                            Quantity =
                                quantityAfter,

                            LastUpdated =
                                DateTime.UtcNow
                        };

                        await _inventoryRepository
                            .AddAsync(inventory);
                    }
                    else
                    {
                        quantityBefore =
                            inventory.Quantity;

                        quantityAfter =
                            quantityBefore +
                            detail.Quantity;

                        inventory.Quantity =
                            quantityAfter;

                        inventory.LastUpdated =
                            DateTime.UtcNow;

                        _inventoryRepository.Update(
                            inventory);
                    }

                    // Ghi log nhập kho do nhận điều chuyển
                    var transferInLog =
                        new InventoryLog
                        {
                            KitchenId = transfer.ToKitchenId,
                            IngredientId = detail.IngredientId,
                            PerformedByAccountId = currentUser.AccountId,
                            LogType = InventoryLogType.TransferIn,
                            QuantityChange = detail.Quantity,
                            QuantityBefore =quantityBefore,
                            QuantityAfter =quantityAfter,
                            Note =$"Transfer in for Transfer Request #{transfer.TransferRequestId}.",
                            CreatedAt =DateTime.UtcNow
                        };

                    await _inventoryLogRepository
                        .AddAsync(transferInLog);
                }

                // Chuyển trạng thái sang Received sau khi cộng đủ kho
                transfer.Status =
                    TransferStatus.Received;

                transfer.ReceivedByAccountId = currentUser.AccountId;

                transfer.ReceivedAt =
                    DateTime.UtcNow;

                _transferRequestRepository
                    .Update(transfer);

                await _unitOfWork.SaveChangesAsync();

                await transaction.CommitAsync();

                return new TransferActionResponse
                {
                    TransferRequestId =
                        transfer.TransferRequestId,

                    Status =
                        transfer.Status.ToString(),

                    ActionAt =
                        transfer.ReceivedAt.Value,

                    Warnings =
                        new List<string>()
                };
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<TransferRequestResponse> CancelAsync(
            int transferRequestId)
        {
            var currentUser =
                _currentUserService.GetCurrentUser();

            if (currentUser.Role != AccountRole.SystemAdmin &&
                currentUser.Role != AccountRole.KitchenManager)
            {
                throw new ForbiddenException(
                    "You do not have permission to cancel transfers.");
            }

            var transfer =
                await _transferRequestRepository
                    .GetByIdAsync(transferRequestId);

            if (transfer == null)
            {
                throw new NotFoundException(
                    "Transfer request does not exist.");
            }

            if (currentUser.Role == AccountRole.KitchenManager)
            {
                var kitchenId =
                    RequireUserKitchen(currentUser);

                if (transfer.FromKitchenId != kitchenId)
                {
                    throw new ForbiddenException(
                        "You cannot cancel another kitchen's transfer.");
                }
            }

            /*
             * Chỉ Draft được hủy.
             * InTransit đã trừ kho nên không được hủy trực tiếp.
             */
            if (transfer.Status != TransferStatus.Draft)
            {
                throw new BusinessException(
                    "Only Draft transfers can be cancelled.");
            }

            transfer.Status =
                TransferStatus.Cancelled;

            _transferRequestRepository.Update(
                transfer);

            await _unitOfWork.SaveChangesAsync();

            return MapToResponse(transfer);
        }

        private static int RequireUserKitchen(
            CurrentUserInfo currentUser)
        {
            // Role vận hành phải được gắn KitchenId trong JWT
            if (!currentUser.KitchenId.HasValue)
            {
                throw new ForbiddenException(
                    "The current account is not assigned to a kitchen.");
            }

            return currentUser.KitchenId.Value;
        }

        private static void EnsureCanAccessTransfer(
            CurrentUserInfo currentUser,
            TransferRequest transfer)
        {
            // Admin xem mọi phiếu
            if (currentUser.Role == AccountRole.SystemAdmin)
            {
                return;
            }

            var kitchenId =
                RequireUserKitchen(currentUser);

            if (currentUser.Role ==
                AccountRole.KitchenManager)
            {
                // KitchenManager chỉ xem phiếu xuất từ bếp của mình
                if (transfer.FromKitchenId != kitchenId)
                {
                    throw new ForbiddenException(
                        "You cannot access this transfer.");
                }

                return;
            }

            if (currentUser.Role ==
                AccountRole.BranchManager)
            {
                // BranchManager chỉ xem phiếu gửi đến quán mình
                if (transfer.ToKitchenId != kitchenId)
                {
                    throw new ForbiddenException(
                        "You cannot access this transfer.");
                }

                return;
            }

            throw new ForbiddenException(
                "You do not have permission to access transfers.");
        }

        private static TransferRequestResponse MapToResponse(
            TransferRequest transfer)
        {
            return new TransferRequestResponse
            {
                TransferRequestId =
                    transfer.TransferRequestId,

                FromKitchenId =
                    transfer.FromKitchenId,

                FromKitchenName =
                    transfer.FromKitchen?.KitchenName ??
                    string.Empty,

                ToKitchenId =
                    transfer.ToKitchenId,

                ToKitchenName =
                    transfer.ToKitchen?.KitchenName ??
                    string.Empty,

                CreatedByAccountId = transfer.CreatedByAccountId,
                CreatedByName = transfer.CreatedByAccount?.FullName ?? "Unknown",
                SentByAccountId = transfer.SentByAccountId,
                SentByName = transfer.SentByAccount?.FullName,
                ReceivedByAccountId = transfer.ReceivedByAccountId,
                ReceivedByName = transfer.ReceivedByAccount?.FullName,

                Status =
                    transfer.Status.ToString(),

                Note =
                    transfer.Note,

                CreatedAt =
                    transfer.CreatedAt,

                SentAt =
                    transfer.SentAt,

                ReceivedAt =
                    transfer.ReceivedAt,

                Items =
                    transfer.TransferRequestDetails
                        .Select(detail =>
                            new TransferDetailResponse
                            {
                                TransferRequestDetailId =
                                    detail.TransferRequestDetailId,

                                IngredientId =
                                    detail.IngredientId,

                                IngredientName =
                                    detail.Ingredient
                                        ?.IngredientName ??
                                    string.Empty,

                                Unit =
                                    detail.Ingredient?.Unit ??
                                    string.Empty,

                                Quantity =
                                    detail.Quantity
                            })
                        .ToList()
            };
        }
    }
}
