using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Models
{
    public class ScheduleLog
    {
        public int ScheduleLogId { get; set; }

        public int ScheduleId { get; set; }

        public string Action { get; set; } = null!;

        public string? OldData { get; set; }

        public string? NewData { get; set; }

        public int PerformedByAccountId { get; set; }

        public DateTime PerformedAt { get; set; } = DateTime.Now;

        public SystemAccount PerformedByAccount { get; set; } = null!;
    }
}
