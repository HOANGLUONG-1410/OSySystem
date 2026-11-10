using BusinessObjects.Models;

namespace Repositories.Interfaces
{
    public interface IScheduleLogRepository
    {
        Task<List<ScheduleLog>> GetAllAsync();
        Task<ScheduleLog?> GetByIdAsync(int logId);
        Task AddAsync(ScheduleLog log);
    }
}
