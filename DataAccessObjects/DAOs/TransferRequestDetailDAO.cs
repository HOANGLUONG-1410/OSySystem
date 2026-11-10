using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Data;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects.DAOs
{
    public class TransferRequestDetailDAO
    {
        private readonly BunBoDbContext _context;

        public TransferRequestDetailDAO(BunBoDbContext context)
        {
            _context = context;
        }

        // Lấy danh sách nguyên liệu trong một phiếu điều chuyển
        // Dùng khi trừ kho bếp tổng hoặc cộng kho chi nhánh
        public async Task<List<TransferRequestDetail>> GetByTransferRequestIdAsync(int transferRequestId)
        {
            return await _context.TransferRequestDetails
                .Include(x => x.Ingredient)
                .Where(x => x.TransferRequestId == transferRequestId)
                .ToListAsync();
        }

        // Thêm nhiều nguyên liệu vào phiếu điều chuyển
        public async Task AddRangeAsync(List<TransferRequestDetail> details)
        {
            await _context.TransferRequestDetails.AddRangeAsync(details);
        }
    }
}
