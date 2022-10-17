using System;
using System.Collections.Generic;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    public partial class Payment
    {
        public Payment()
        {
            WalletTransactions = new HashSet<WalletTransaction>();
        }

        public Guid Id { get; set; }
        public Guid? PeriodRevenueId { get; set; }
        public Guid? InvestmentId { get; set; }
        public double? Amount { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public Guid? FromId { get; set; }
        public Guid? ToId { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public string Status { get; set; }
        public Guid? PackageId { get; set; }
        public Guid? StageId { get; set; }

        public virtual Investment Investment { get; set; }
        public virtual PeriodRevenue PeriodRevenue { get; set; }
        public virtual ICollection<WalletTransaction> WalletTransactions { get; set; }
    }
}
