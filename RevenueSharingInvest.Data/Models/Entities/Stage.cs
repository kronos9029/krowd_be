using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    [Table("Stage")]
    [Index(nameof(ProjectId), Name = "IX_Stage_ProjectId")]
    public partial class Stage
    {
        public Stage()
        {
            DailyReports = new HashSet<DailyReport>();
            PeriodRevenues = new HashSet<PeriodRevenue>();
        }

        [Key]
        public Guid Id { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
        public Guid ProjectId { get; set; }
        public string Description { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime EndDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime StartDate { get; set; }
        [Required]
        [StringLength(8)]
        public string Status { get; set; }
        [StringLength(5)]
        public string IsOverDue { get; set; }

        [ForeignKey(nameof(ProjectId))]
        [InverseProperty("Stages")]
        public virtual Project Project { get; set; }
        [InverseProperty(nameof(DailyReport.Stage))]
        public virtual ICollection<DailyReport> DailyReports { get; set; }
        [InverseProperty(nameof(PeriodRevenue.Stage))]
        public virtual ICollection<PeriodRevenue> PeriodRevenues { get; set; }
    }
}
