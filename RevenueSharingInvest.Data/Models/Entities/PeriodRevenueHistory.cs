using System;
using System.Collections.Generic;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    public partial class PeriodRevenueHistory
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? PeriodRevenueId { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
        public double? Amount { get; set; }
        public double? StageTotalAmount { get; set; }

        public virtual PeriodRevenue PeriodRevenue { get; set; }
    }
}
