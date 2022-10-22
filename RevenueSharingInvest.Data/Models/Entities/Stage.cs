using System;
using System.Collections.Generic;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    public partial class Stage
    {
        public Stage()
        {
            DailyReports = new HashSet<DailyReport>();
            PeriodRevenues = new HashSet<PeriodRevenue>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid ProjectId { get; set; }
        public string Description { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime StartDate { get; set; }
        public string Status { get; set; }
        public string IsOverDue { get; set; }

        public virtual Project Project { get; set; }
        public virtual ICollection<DailyReport> DailyReports { get; set; }
        public virtual ICollection<PeriodRevenue> PeriodRevenues { get; set; }
    }
}
