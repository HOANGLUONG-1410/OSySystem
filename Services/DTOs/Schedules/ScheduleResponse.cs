namespace Services.DTOs.Schedules
{
    public class ScheduleResponse
    {
        public int ScheduleId { get; set; }
        public int KitchenId { get; set; }
        public string KitchenName { get; set; } = null!;
        public int StaffAccountId { get; set; }
        public string StaffName { get; set; } = null!;
        public string WorkDate { get; set; } = null!;
        public string ShiftType { get; set; } = null!;
        public string? Note { get; set; }
        public int CreatedByAccountId { get; set; }
        public string CreatedByName { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
