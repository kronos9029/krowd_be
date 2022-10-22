using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    public partial class DailyReport
    {
        public DailyReport()
        {
            Bills = new HashSet<Bill>();
        }

        public Guid Id { get; set; }
        public Guid StageId { get; set; }
        public double Amount { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime ReportDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string? UpdateBy { get; set; }
        [StringLength(20)]
        public string Status { get; set; }

        public virtual Stage Stage { get; set; }
        public virtual ICollection<Bill> Bills { get; set; }
    }
}
