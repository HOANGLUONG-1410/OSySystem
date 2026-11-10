using BusinessObjects.Models;
using DataAccessObjects.DAOs;
using Repositories.Interfaces;

namespace Repositories.Implementations
{
    public class ScheduleLogRepository : IScheduleLogRepository
    {
        private readonly ScheduleLogDAO _scheduleLogDAO;

        public ScheduleLogRepository(ScheduleLogDAO scheduleLogDAO)
        {
            _scheduleLogDAO = scheduleLogDAO;
        }

        public Task<List<ScheduleLog>> GetAllAsync() => _scheduleLogDAO.GetAllAsync();
        public Task<ScheduleLog?> GetByIdAsync(int logId) => _scheduleLogDAO.GetByIdAsync(logId);
        public Task AddAsync(ScheduleLog log) => _scheduleLogDAO.AddAsync(log);
    }
}
