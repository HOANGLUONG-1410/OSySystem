using BusinessObjects.Data;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects.DAOs
{
    public class ScheduleLogDAO
    {
        private readonly BunBoDbContext _context;

        public ScheduleLogDAO(BunBoDbContext context)
        {
            _context = context;
        }

        public async Task<List<ScheduleLog>> GetAllAsync()
        {
            return await _context.Set<ScheduleLog>()
                .AsNoTracking()
                .Include(x => x.PerformedByAccount)
                .OrderByDescending(x => x.PerformedAt)
                .ToListAsync();
        }

        public async Task<ScheduleLog?> GetByIdAsync(int logId)
        {
            return await _context.Set<ScheduleLog>()
                .AsNoTracking()
                .Include(x => x.PerformedByAccount)
                .FirstOrDefaultAsync(x => x.ScheduleLogId == logId);
        }

        public async Task AddAsync(ScheduleLog log)
        {
            await _context.Set<ScheduleLog>().AddAsync(log);
        }
    }
}
