namespace Services.DTOs.Schedules
{
    public class ScheduleLogDetailResponse
    {
        public int ScheduleLogId { get; set; }
        public int ScheduleId { get; set; }
        public string Action { get; set; } = null!;
        public string PerformedByName { get; set; } = null!;
        public DateTime PerformedAt { get; set; }
        public List<FieldChange> Changes { get; set; } = new();
    }

    public class FieldChange
    {
        public string FieldName { get; set; } = null!;
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
    }
}
