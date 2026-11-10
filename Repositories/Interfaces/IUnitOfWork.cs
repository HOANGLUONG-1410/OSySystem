using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        // Lưu toàn bộ thay đổi đang được DbContext theo dõi
        Task<int> SaveChangesAsync();

        // Bắt đầu transaction cho các nghiệp vụ nhiều bước
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}
