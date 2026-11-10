using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Models
{
    public class Schedule
    {
        public int ScheduleId { get; set; }

        public int KitchenId { get; set; }

        public int StaffAccountId { get; set; }

        public DateTime WorkDate { get; set; }

        public string ShiftType { get; set; } = null!;

        public string? Note { get; set; }

        public int CreatedByAccountId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        public Kitchen Kitchen { get; set; } = null!;

        public SystemAccount StaffAccount { get; set; } = null!;

        public SystemAccount CreatedByAccount { get; set; } = null!;
    }
}
