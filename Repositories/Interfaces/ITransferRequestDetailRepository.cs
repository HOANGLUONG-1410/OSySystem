using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;

namespace Repositories.Interfaces
{
    public interface ITransferRequestDetailRepository
    {
        // Lấy các nguyên liệu thuộc một phiếu điều chuyển
        Task<List<TransferRequestDetail>>
            GetByTransferRequestIdAsync(
                int transferRequestId);

        // Thêm nhiều dòng nguyên liệu vào phiếu
        Task AddRangeAsync(
            List<TransferRequestDetail> details);
    }
}
