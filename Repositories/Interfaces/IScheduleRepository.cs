using BusinessObjects.Models;

namespace Repositories.Interfaces
{
    public interface IScheduleRepository
    {
        Task<List<Schedule>> GetAllAsync();
        Task<Schedule?> GetByIdAsync(int scheduleId);
        Task<List<Schedule>> GetByKitchenIdAsync(int kitchenId);
        Task<List<Schedule>> GetByStaffAccountIdAsync(int staffAccountId);
        Task<List<Schedule>> GetByKitchenAndDateRangeAsync(int kitchenId, DateTime from, DateTime to);
        Task AddAsync(Schedule schedule);
        void Update(Schedule schedule);
        void Delete(Schedule schedule);
    }
}
