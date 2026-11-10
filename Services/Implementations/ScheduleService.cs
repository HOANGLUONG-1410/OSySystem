using System.Text.Json;
using BusinessObjects.Enums;
using BusinessObjects.Models;
using Repositories.Interfaces;
using Services.DTOs.Schedules;
using Services.Exceptions;
using Services.Interfaces;

namespace Services.Implementations
{
    public class ScheduleService : IScheduleService
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IScheduleLogRepository _scheduleLogRepository;
        private readonly ISystemAccountRepository _accountRepository;
        private readonly IKitchenRepository _kitchenRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public ScheduleService(
            IScheduleRepository scheduleRepository,
            IScheduleLogRepository scheduleLogRepository,
            ISystemAccountRepository accountRepository,
            IKitchenRepository kitchenRepository,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork)
        {
            _scheduleRepository = scheduleRepository;
            _scheduleLogRepository = scheduleLogRepository;
            _accountRepository = accountRepository;
            _kitchenRepository = kitchenRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ScheduleResponse>> GetByKitchenIdAsync(int kitchenId)
        {
            var schedules = await _scheduleRepository.GetByKitchenIdAsync(kitchenId);
            return schedules.Select(MapToResponse).ToList();
        }

        public async Task<List<ScheduleResponse>> GetMySchedulesAsync()
        {
            var currentUser = _currentUserService.GetCurrentUser();
            var schedules = await _scheduleRepository.GetByStaffAccountIdAsync(currentUser.AccountId);
            return schedules.Select(MapToResponse).ToList();
        }

        public async Task<ScheduleResponse> CreateAsync(CreateScheduleRequest request)
        {
            var currentUser = _currentUserService.GetCurrentUser();

            if (currentUser.Role != AccountRole.BranchManager && currentUser.Role != AccountRole.SystemAdmin)
                throw new ForbiddenException("Only BranchManager and Admin can create schedules.");

            var kitchenId = currentUser.Role == AccountRole.SystemAdmin
                ? (await _accountRepository.GetByIdAsync(request.StaffAccountId))?.KitchenId
                : currentUser.KitchenId;

            if (!kitchenId.HasValue)
                throw new BusinessException("Could not determine kitchen for this schedule.");

            var staff = await _accountRepository.GetByIdAsync(request.StaffAccountId);
            if (staff == null || !staff.IsActive)
                throw new BusinessException("Staff account does not exist or is inactive.");

            if (staff.Role != AccountRole.Staff)
                throw new BusinessException("Only Staff accounts can be assigned schedules.");

            // Kiểm tra trùng: nhân viên đã có ca này trong ngày chưa
            var existingSchedules = await _scheduleRepository
                .GetByStaffAccountIdAsync(request.StaffAccountId);

            var duplicate = existingSchedules.Any(s =>
                s.WorkDate.Date == request.WorkDate.Date
                && s.ShiftType == request.ShiftType);

            if (duplicate)
                throw new BusinessException(
                    $"Nhân viên {staff.FullName} đã có ca {request.ShiftType} "
                    + $"vào ngày {request.WorkDate:dd/MM/yyyy}.");

            var schedule = new Schedule
            {
                KitchenId = kitchenId.Value,
                StaffAccountId = request.StaffAccountId,
                WorkDate = request.WorkDate,
                ShiftType = request.ShiftType,
                Note = request.Note,
                CreatedByAccountId = currentUser.AccountId,
                CreatedAt = DateTime.UtcNow
            };

            await _scheduleRepository.AddAsync(schedule);
            await _unitOfWork.SaveChangesAsync();

            await LogAsync(schedule.ScheduleId, "Created", null, schedule, currentUser.AccountId);

            return MapToResponse(schedule);
        }

        public async Task<ScheduleResponse> UpdateAsync(int scheduleId, UpdateScheduleRequest request)
        {
            var currentUser = _currentUserService.GetCurrentUser();

            if (currentUser.Role != AccountRole.BranchManager && currentUser.Role != AccountRole.SystemAdmin)
                throw new ForbiddenException("Only BranchManager and Admin can update schedules.");

            var schedule = await _scheduleRepository.GetByIdAsync(scheduleId);
            if (schedule == null)
                throw new NotFoundException("Schedule does not exist.");

            if (currentUser.Role == AccountRole.BranchManager && schedule.KitchenId != currentUser.KitchenId)
                throw new ForbiddenException("You cannot update schedules from another kitchen.");

            // Kiểm tra trùng: nhân viên đã có ca này trong ngày chưa
            var existingSchedules = await _scheduleRepository
                .GetByStaffAccountIdAsync(schedule.StaffAccountId);

            var duplicate = existingSchedules.Any(s =>
                s.ScheduleId != scheduleId
                && s.WorkDate.Date == request.WorkDate.Date
                && s.ShiftType == request.ShiftType);

            if (duplicate)
                throw new BusinessException(
                    $"Nhân viên {schedule.StaffAccount?.FullName ?? "này"} đã có ca {request.ShiftType} "
                    + $"vào ngày {request.WorkDate:dd/MM/yyyy}.");

            var oldData = JsonSerializer.Serialize(MapToResponse(schedule));

            schedule.WorkDate = request.WorkDate;
            schedule.ShiftType = request.ShiftType;
            schedule.Note = request.Note;
            schedule.UpdatedAt = DateTime.UtcNow;

            _scheduleRepository.Update(schedule);
            await _unitOfWork.SaveChangesAsync();

            await LogAsync(scheduleId, "Updated", oldData, schedule, currentUser.AccountId);

            return MapToResponse(schedule);
        }

        public async Task DeleteAsync(int scheduleId)
        {
            var currentUser = _currentUserService.GetCurrentUser();

            if (currentUser.Role != AccountRole.BranchManager && currentUser.Role != AccountRole.SystemAdmin)
                throw new ForbiddenException("Only BranchManager and Admin can delete schedules.");

            var schedule = await _scheduleRepository.GetByIdAsync(scheduleId);
            if (schedule == null)
                throw new NotFoundException("Schedule does not exist.");

            if (currentUser.Role == AccountRole.BranchManager && schedule.KitchenId != currentUser.KitchenId)
                throw new ForbiddenException("You cannot delete schedules from another kitchen.");

            var oldData = JsonSerializer.Serialize(MapToResponse(schedule));

            // Ghi log trước khi xóa để đảm bảo audit trail luôn được lưu
            await LogAsync(scheduleId, "Deleted", oldData, null, currentUser.AccountId);

            _scheduleRepository.Delete(schedule);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<ScheduleLogResponse>> GetLogsAsync()
        {
            var logs = await _scheduleLogRepository.GetAllAsync();
            return logs.Select(l => new ScheduleLogResponse
            {
                ScheduleLogId = l.ScheduleLogId,
                ScheduleId = l.ScheduleId,
                Action = l.Action,
                OldData = l.OldData,
                NewData = l.NewData,
                PerformedByAccountId = l.PerformedByAccountId,
                PerformedByName = l.PerformedByAccount?.FullName ?? "Unknown",
                PerformedAt = l.PerformedAt
            }).ToList();
        }

        public async Task<ScheduleLogDetailResponse?> GetLogDetailAsync(int logId)
        {
            var log = await _scheduleLogRepository.GetByIdAsync(logId);
            if (log == null) return null;

            var result = new ScheduleLogDetailResponse
            {
                ScheduleLogId = log.ScheduleLogId,
                ScheduleId = log.ScheduleId,
                Action = log.Action,
                PerformedByName = log.PerformedByAccount?.FullName ?? "Unknown",
                PerformedAt = log.PerformedAt,
                Changes = ComputeDiff(log.OldData, log.NewData)
            };

            return result;
        }

        private static List<FieldChange> ComputeDiff(string? oldJson, string? newJson)
        {
            var changes = new List<FieldChange>();
            var oldDict = ParseJson(oldJson);
            var newDict = ParseJson(newJson);

            var allKeys = new HashSet<string>();
            if (oldDict != null)
                foreach (var k in oldDict.Keys) allKeys.Add(k);
            if (newDict != null)
                foreach (var k in newDict.Keys) allKeys.Add(k);

            var fieldNames = new Dictionary<string, string>
            {
                ["staffName"] = "Nhân viên",
                ["shiftType"] = "Ca",
                ["workDate"] = "Ngày làm",
                ["note"] = "Ghi chú",
                ["kitchenName"] = "Chi nhánh",
                ["createdByName"] = "Người tạo"
            };

            foreach (var key in allKeys.OrderBy(x => x))
            {
                var oldVal = oldDict?.GetValueOrDefault(key)?.ToString();
                var newVal = newDict?.GetValueOrDefault(key)?.ToString();

                if (oldVal != newVal)
                {
                    changes.Add(new FieldChange
                    {
                        FieldName = fieldNames.GetValueOrDefault(key, key),
                        OldValue = oldVal,
                        NewValue = newVal
                    });
                }
            }

            return changes;
        }

        private static Dictionary<string, object?>? ParseJson(string? json)
        {
            if (string.IsNullOrWhiteSpace(json)) return null;
            try
            {
                return JsonSerializer.Deserialize<Dictionary<string, object?>>(json);
            }
            catch
            {
                return null;
            }
        }

        private async Task LogAsync(int scheduleId, string action, string? oldData, Schedule? newSchedule, int performedBy)
        {
            var log = new ScheduleLog
            {
                ScheduleId = scheduleId,
                Action = action,
                OldData = oldData,
                NewData = newSchedule != null ? JsonSerializer.Serialize(MapToResponse(newSchedule)) : null,
                PerformedByAccountId = performedBy,
                PerformedAt = DateTime.UtcNow
            };
            await _scheduleLogRepository.AddAsync(log);
            await _unitOfWork.SaveChangesAsync();
        }

        private static ScheduleResponse MapToResponse(Schedule s)
        {
            return new ScheduleResponse
            {
                ScheduleId = s.ScheduleId,
                KitchenId = s.KitchenId,
                KitchenName = s.Kitchen?.KitchenName ?? string.Empty,
                StaffAccountId = s.StaffAccountId,
                StaffName = s.StaffAccount?.FullName ?? "Unknown",
                WorkDate = s.WorkDate.ToString("yyyy-MM-dd"),
                ShiftType = s.ShiftType,
                Note = s.Note,
                CreatedByAccountId = s.CreatedByAccountId,
                CreatedByName = s.CreatedByAccount?.FullName ?? "Unknown",
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            };
        }
    }
}
