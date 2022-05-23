﻿using System;
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

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }
        public Guid? InvestmentId { get; set; }
        public Guid? PeriodRevenueId { get; set; }
        public double? Amount { get; set; }
        public string Description { get; set; }
        [StringLength(20)]
        public string Type { get; set; }
        public Guid? FromId { get; set; }
        public Guid? ToId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
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
