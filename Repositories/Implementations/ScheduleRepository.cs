using BusinessObjects.Models;
using DataAccessObjects.DAOs;
using Repositories.Interfaces;

namespace Repositories.Implementations
{
    public class ScheduleRepository : IScheduleRepository
    {
        private readonly ScheduleDAO _scheduleDAO;

        public ScheduleRepository(ScheduleDAO scheduleDAO)
        {
            _scheduleDAO = scheduleDAO;
        }

        public Task<List<Schedule>> GetAllAsync() => _scheduleDAO.GetAllAsync();
        public Task<Schedule?> GetByIdAsync(int scheduleId) => _scheduleDAO.GetByIdAsync(scheduleId);
        public Task<List<Schedule>> GetByKitchenIdAsync(int kitchenId) => _scheduleDAO.GetByKitchenIdAsync(kitchenId);
        public Task<List<Schedule>> GetByStaffAccountIdAsync(int staffAccountId) => _scheduleDAO.GetByStaffAccountIdAsync(staffAccountId);
        public Task<List<Schedule>> GetByKitchenAndDateRangeAsync(int kitchenId, DateTime from, DateTime to) => _scheduleDAO.GetByKitchenAndDateRangeAsync(kitchenId, from, to);
        public Task AddAsync(Schedule schedule) => _scheduleDAO.AddAsync(schedule);
        public void Update(Schedule schedule) => _scheduleDAO.Update(schedule);
        public void Delete(Schedule schedule) => _scheduleDAO.Delete(schedule);
    }
}
