using System.ComponentModel.DataAnnotations;

namespace Services.DTOs.Schedules
{
    public class UpdateScheduleRequest
    {
        [Required]
        public DateTime WorkDate { get; set; }

        [Required]
        [MaxLength(20)]
        public string ShiftType { get; set; } = null!;

        [MaxLength(500)]
        public string? Note { get; set; }
    }
}
