using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    [Table("Stage")]
    public partial class Stage
    {
        public Stage()
        {
            PeriodRevenues = new HashSet<PeriodRevenue>();
        }

        [Key]
        public Guid Id { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
        public Guid? ProjectId { get; set; }
        public string Description { get; set; }
        public double? Percents { get; set; }
        public int? OpenMonth { get; set; }
        public int? CloseMonth { get; set; }
        [StringLength(20)]
        public string Status { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
        public bool? IsDeleted { get; set; }

        [ForeignKey(nameof(ProjectId))]
        [InverseProperty("Stages")]
        public virtual Project Project { get; set; }
        [InverseProperty(nameof(PeriodRevenue.Stage))]
        public virtual ICollection<PeriodRevenue> PeriodRevenues { get; set; }
    }
}
