using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Data;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects.DAOs
{
    public class TransferRequestDAO
    {
        private readonly BunBoDbContext _context;

        public TransferRequestDAO(BunBoDbContext context)
        {
            _context = context;
        }

        // Lấy toàn bộ phiếu điều chuyển trong hệ thống
        // Dùng cho SystemAdmin xem và giám sát toàn bộ chuỗi
        public async Task<List<TransferRequest>> GetAllAsync()
        {
            return await _context.TransferRequests
                .Include(x => x.FromKitchen)
                .Include(x => x.ToKitchen)
                .Include(x => x.TransferRequestDetails)
                    .ThenInclude(x => x.Ingredient)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Lấy một phiếu điều chuyển theo id
        // Dùng khi xem chi tiết, duyệt xuất kho hoặc xác nhận nhận hàng
        public async Task<TransferRequest?> GetByIdAsync(int id)
        {
            return await _context.TransferRequests
                .Include(x => x.FromKitchen)
                .Include(x => x.ToKitchen)
                .Include(x => x.TransferRequestDetails)
                    .ThenInclude(x => x.Ingredient)
                .FirstOrDefaultAsync(
                    x => x.TransferRequestId == id);
        }

        // Lấy các phiếu gửi đi từ một Kitchen
        // Thường dùng cho Bếp tổng xem các phiếu đã tạo
        public async Task<List<TransferRequest>> GetByFromKitchenIdAsync(
            int kitchenId)
        {
            return await _context.TransferRequests
                .Include(x => x.FromKitchen)
                .Include(x => x.ToKitchen)
                .Include(x => x.TransferRequestDetails)
                    .ThenInclude(x => x.Ingredient)
                .Where(x => x.FromKitchenId == kitchenId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Lấy các phiếu được gửi đến một Kitchen
        // Dùng cho BranchManager xem hàng đang chuyển đến quán mình
        public async Task<List<TransferRequest>> GetByToKitchenIdAsync(
            int kitchenId)
        {
            return await _context.TransferRequests
                .Include(x => x.FromKitchen)
                .Include(x => x.ToKitchen)
                .Include(x => x.TransferRequestDetails)
                    .ThenInclude(x => x.Ingredient)
                .Where(x => x.ToKitchenId == kitchenId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        // Lấy phiếu theo Kitchen nhận và trạng thái
        // Ví dụ BranchManager lấy các phiếu InTransit đang chờ nhận
        public async Task<List<TransferRequest>>
            GetByToKitchenAndStatusAsync(
                int kitchenId,
                TransferStatus status)
        {
            return await _context.TransferRequests
                .Include(x => x.FromKitchen)
                .Include(x => x.ToKitchen)
                .Include(x => x.TransferRequestDetails)
                    .ThenInclude(x => x.Ingredient)
                .Where(x =>
                    x.ToKitchenId == kitchenId &&
                    x.Status == status)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();
        }

        // Thêm phiếu điều chuyển mới
        // Trạng thái ban đầu luôn là Draft
        public async Task AddAsync(
            TransferRequest transferRequest)
        {
            await _context.TransferRequests.AddAsync(
                transferRequest);
        }

        // Cập nhật trạng thái và thời gian của phiếu
        // Ví dụ Draft -> InTransit hoặc InTransit -> Received
        public void Update(
            TransferRequest transferRequest)
        {
            _context.TransferRequests.Update(
                transferRequest);
        }
    }
}
