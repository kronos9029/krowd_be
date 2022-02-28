using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    [Table("PeriodRevenueHistory")]
    public partial class PeriodRevenueHistory
    {
        [Key]
        [Column("ID")]
        [StringLength(10)]
        public string Id { get; set; }
        [Column("name")]
        [StringLength(50)]
        public string Name { get; set; }
        [Column("periodRevenueID")]
        [StringLength(10)]
        public string PeriodRevenueId { get; set; }
        [Column("description")]
        public string Description { get; set; }
        [Column("status")]
        [StringLength(20)]
        public string Status { get; set; }
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

        [ForeignKey(nameof(PeriodRevenueId))]
        [InverseProperty("PeriodRevenueHistories")]
        public virtual PeriodRevenue PeriodRevenue { get; set; }
    }
}
