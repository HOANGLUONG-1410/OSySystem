using Services.DTOs.Schedules;

namespace Services.Interfaces
{
    public interface IScheduleService
    {
        Task<List<ScheduleResponse>> GetByKitchenIdAsync(int kitchenId);
        Task<List<ScheduleResponse>> GetMySchedulesAsync();
        Task<ScheduleResponse> CreateAsync(CreateScheduleRequest request);
        Task<ScheduleResponse> UpdateAsync(int scheduleId, UpdateScheduleRequest request);
        Task DeleteAsync(int scheduleId);
        Task<List<ScheduleLogResponse>> GetLogsAsync();
        Task<ScheduleLogDetailResponse?> GetLogDetailAsync(int logId);
    }
}
