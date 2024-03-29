﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    [Table("PeriodRevenue")]
    [Index(nameof(ProjectId), Name = "IX_PeriodRevenue_ProjectId")]
    [Index(nameof(StageId), Name = "IX_PeriodRevenue_StageId")]
    public partial class PeriodRevenue
    {
        public PeriodRevenue()
        {
            Payments = new HashSet<Payment>();
            PeriodRevenueHistories = new HashSet<PeriodRevenueHistory>();
        }

        [Key]
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public Guid StageId { get; set; }
        public double? ActualAmount { get; set; }
        public double? SharedAmount { get; set; }
        public double? PaidAmount { get; set; }
        public double? PessimisticExpectedAmount { get; set; }
        public double? OptimisticExpectedAmount { get; set; }
        public double? NormalExpectedAmount { get; set; }
        public double? PessimisticExpectedRatio { get; set; }
        public double? OptimisticExpectedRatio { get; set; }
        public double? NormalExpectedRatio { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
        [StringLength(15)]
        public string Status { get; set; }

        [ForeignKey(nameof(ProjectId))]
        [InverseProperty("PeriodRevenues")]
        public virtual Project Project { get; set; }
        [ForeignKey(nameof(StageId))]
        [InverseProperty("PeriodRevenues")]
        public virtual Stage Stage { get; set; }
        [InverseProperty(nameof(Payment.PeriodRevenue))]
        public virtual ICollection<Payment> Payments { get; set; }
        [InverseProperty(nameof(PeriodRevenueHistory.PeriodRevenue))]
        public virtual ICollection<PeriodRevenueHistory> PeriodRevenueHistories { get; set; }
    }
}
