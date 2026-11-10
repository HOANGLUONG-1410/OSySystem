using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects.Models;
using DataAccessObjects.DAOs;
using Repositories.Interfaces;

namespace Repositories.Implementations
{
    public class TransferRequestDetailRepository
        : ITransferRequestDetailRepository
    {
        private readonly TransferRequestDetailDAO
            _transferRequestDetailDAO;

        public TransferRequestDetailRepository(
            TransferRequestDetailDAO transferRequestDetailDAO)
        {
            _transferRequestDetailDAO =
                transferRequestDetailDAO;
        }

        // Lấy toàn bộ nguyên liệu trong một phiếu
        public Task<List<TransferRequestDetail>>
            GetByTransferRequestIdAsync(
                int transferRequestId)
        {
            return _transferRequestDetailDAO
                .GetByTransferRequestIdAsync(
                    transferRequestId);
        }

        // Thêm nhiều chi tiết trong một lần
        public Task AddRangeAsync(
            List<TransferRequestDetail> details)
        {
            return _transferRequestDetailDAO
                .AddRangeAsync(details);
        }
    }
}
