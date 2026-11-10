using BusinessObjects.Data;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects.DAOs
{
    public class ScheduleDAO
    {
        private readonly BunBoDbContext _context;

        public ScheduleDAO(BunBoDbContext context)
        {
            _context = context;
        }

        public async Task<List<Schedule>> GetAllAsync()
        {
            return await _context.Set<Schedule>()
                .AsNoTracking()
                .Include(x => x.Kitchen)
                .Include(x => x.StaffAccount)
                .Include(x => x.CreatedByAccount)
                .OrderByDescending(x => x.WorkDate)
                .ToListAsync();
        }

        public async Task<Schedule?> GetByIdAsync(int id)
        {
            return await _context.Set<Schedule>()
                .AsNoTracking()
                .Include(x => x.Kitchen)
                .Include(x => x.StaffAccount)
                .Include(x => x.CreatedByAccount)
                .FirstOrDefaultAsync(x => x.ScheduleId == id);
        }

        public async Task<List<Schedule>> GetByKitchenIdAsync(int kitchenId)
        {
            return await _context.Set<Schedule>()
                .AsNoTracking()
                .Include(x => x.Kitchen)
                .Include(x => x.StaffAccount)
                .Include(x => x.CreatedByAccount)
                .Where(x => x.KitchenId == kitchenId)
                .OrderByDescending(x => x.WorkDate)
                .ToListAsync();
        }

        public async Task<List<Schedule>> GetByStaffAccountIdAsync(int staffAccountId)
        {
            return await _context.Set<Schedule>()
                .AsNoTracking()
                .Include(x => x.Kitchen)
                .Include(x => x.StaffAccount)
                .Include(x => x.CreatedByAccount)
                .Where(x => x.StaffAccountId == staffAccountId)
                .OrderByDescending(x => x.WorkDate)
                .ToListAsync();
        }

        public async Task<List<Schedule>> GetByKitchenAndDateRangeAsync(int kitchenId, DateTime from, DateTime to)
        {
            return await _context.Set<Schedule>()
                .AsNoTracking()
                .Include(x => x.StaffAccount)
                .Include(x => x.CreatedByAccount)
                .Where(x => x.KitchenId == kitchenId && x.WorkDate >= from && x.WorkDate <= to)
                .OrderBy(x => x.WorkDate)
                .ToListAsync();
        }

        public async Task AddAsync(Schedule schedule)
        {
            await _context.Set<Schedule>().AddAsync(schedule);
        }

        public void Update(Schedule schedule)
        {
            _context.Set<Schedule>().Update(schedule);
        }

        public void Delete(Schedule schedule)
        {
            _context.Set<Schedule>().Remove(schedule);
        }
    }
}
