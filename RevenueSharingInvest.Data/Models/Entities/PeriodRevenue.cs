using System;
using System.Collections.Generic;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    public partial class PeriodRevenue
    {
        public PeriodRevenue()
        {
            Payments = new HashSet<Payment>();
            PeriodRevenueHistories = new HashSet<PeriodRevenueHistory>();
        }

        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public Guid StageId { get; set; }
        public double? ActualAmount { get; set; }
        public double? PessimisticExpectedAmount { get; set; }
        public double? OptimisticExpectedAmount { get; set; }
        public double? NormalExpectedAmount { get; set; }
        public double? PessimisticExpectedRatio { get; set; }
        public double? OptimisticExpectedRatio { get; set; }
        public double? NormalExpectedRatio { get; set; }
        public string Status { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        public virtual Project Project { get; set; }
        public virtual Stage Stage { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
        public virtual ICollection<PeriodRevenueHistory> PeriodRevenueHistories { get; set; }
    }
}
