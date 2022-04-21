using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    [Table("Payment")]
    public partial class Payment
    {
        public Payment()
        {
            WalletTransactions = new HashSet<WalletTransaction>();
        }

        [Key]
        public Guid Id { get; set; }
        public Guid? InvestmentId { get; set; }
        public Guid? PeriodRevenueId { get; set; }
        public double? Amount { get; set; }
        public string Description { get; set; }
        [StringLength(20)]
        public string Type { get; set; }
        [StringLength(10)]
        public string FromId { get; set; }
        [StringLength(10)]
        public string ToId { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
        public bool? IsDeleted { get; set; }

        [ForeignKey(nameof(InvestmentId))]
        [InverseProperty("Payments")]
        public virtual Investment Investment { get; set; }
        [ForeignKey(nameof(PeriodRevenueId))]
        [InverseProperty("Payments")]
        public virtual PeriodRevenue PeriodRevenue { get; set; }
        [InverseProperty(nameof(WalletTransaction.Payment))]
        public virtual ICollection<WalletTransaction> WalletTransactions { get; set; }
    }
}
