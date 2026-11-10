namespace Services.DTOs.Schedules
{
    public class ScheduleLogResponse
    {
        public int ScheduleLogId { get; set; }
        public int ScheduleId { get; set; }
        public string Action { get; set; } = null!;
        public string? OldData { get; set; }
        public string? NewData { get; set; }
        public int PerformedByAccountId { get; set; }
        public string PerformedByName { get; set; } = null!;
        public DateTime PerformedAt { get; set; }
    }
}
