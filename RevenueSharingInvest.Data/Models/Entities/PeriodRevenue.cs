using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    [Table("PeriodRevenue")]
    public partial class PeriodRevenue
    {
        public PeriodRevenue()
        {
            Payments = new HashSet<Payment>();
            PeriodRevenueHistories = new HashSet<PeriodRevenueHistory>();
        }

        [Key]
        [Column("ID")]
        [StringLength(10)]
        public string Id { get; set; }
        [Column("projectID")]
        [StringLength(10)]
        public string ProjectId { get; set; }
        [Column("periodNum")]
        public int? PeriodNum { get; set; }
        [Column("amount")]
        public double? Amount { get; set; }
        [Column("createDate", TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        [Column("createBy")]
        [StringLength(10)]
        public string CreateBy { get; set; }
        [Column("updateDate", TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        [Column("updateBy")]
        [StringLength(10)]
        public string UpdateBy { get; set; }
        [Column("isDeleted")]
        public bool? IsDeleted { get; set; }

        [ForeignKey(nameof(ProjectId))]
        [InverseProperty("PeriodRevenues")]
        public virtual Project Project { get; set; }
        [InverseProperty(nameof(Payment.PeriodRevenue))]
        public virtual ICollection<Payment> Payments { get; set; }
        [InverseProperty(nameof(PeriodRevenueHistory.PeriodRevenue))]
        public virtual ICollection<PeriodRevenueHistory> PeriodRevenueHistories { get; set; }
    }
}
