using BusinessObjects.Data;
using Microsoft.EntityFrameworkCore.Storage;
using Repositories.Interfaces;

namespace Repositories.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BunBoDbContext _context;

        public UnitOfWork(BunBoDbContext context)
        {
            _context = context;
        }

        // Commit các thao tác Add, Update, Remove xuống database
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        // Mở transaction để Service đảm bảo tính nguyên tử
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }
    }
}
