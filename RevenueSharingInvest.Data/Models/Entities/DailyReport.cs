using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    [Keyless]
    [Table("DailyReport")]
    public partial class DailyReport
    {
        public Guid Id { get; set; }
        public Guid StageId { get; set; }
        public double? Amount { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ReportDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
        [StringLength(20)]
        public string Status { get; set; }

        [ForeignKey(nameof(Id))]
        public virtual Bill IdNavigation { get; set; }
        [ForeignKey(nameof(StageId))]
        public virtual Stage Stage { get; set; }
    }
}
