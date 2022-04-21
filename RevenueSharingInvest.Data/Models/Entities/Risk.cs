using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    [Table("Risk")]
    public partial class Risk
    {
        [Key]
        public Guid Id { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
        public Guid? ProjectId { get; set; }
        public Guid? RiskTypeId { get; set; }
        public string Description { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
        public bool? IsDeleted { get; set; }

        [ForeignKey(nameof(ProjectId))]
        [InverseProperty("Risks")]
        public virtual Project Project { get; set; }
        [ForeignKey(nameof(RiskTypeId))]
        [InverseProperty("Risks")]
        public virtual RiskType RiskType { get; set; }
    }
}
