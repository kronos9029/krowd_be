using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    [Table("DailyReport")]
    public partial class DailyReport
    {
        public DailyReport()
        {
            Bills = new HashSet<Bill>();
        }

        [Key]
        public Guid Id { get; set; }
        public Guid StageId { get; set; }
        public double Amount { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime ReportDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public string? UpdateBy { get; set; }
        [StringLength(20)]
        public string Status { get; set; }

        [ForeignKey(nameof(StageId))]
        [InverseProperty("DailyReports")]
        public virtual Stage Stage { get; set; }
        [InverseProperty(nameof(Bill.DailyReport))]
        public virtual ICollection<Bill> Bills { get; set; }
    }
}
