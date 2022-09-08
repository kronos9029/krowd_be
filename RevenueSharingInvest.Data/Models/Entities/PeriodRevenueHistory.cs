using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    [Table("PeriodRevenueHistory")]
    [Index(nameof(PeriodRevenueId), Name = "IX_PeriodRevenueHistory_PeriodRevenueId")]
    public partial class PeriodRevenueHistory
    {
        [Key]
        public Guid Id { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
        public Guid? PeriodRevenueId { get; set; }
        public string Description { get; set; }
        [StringLength(20)]
        public string Status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
        public bool? IsDeleted { get; set; }

        [ForeignKey(nameof(PeriodRevenueId))]
        [InverseProperty("PeriodRevenueHistories")]
        public virtual PeriodRevenue PeriodRevenue { get; set; }
    }
}
