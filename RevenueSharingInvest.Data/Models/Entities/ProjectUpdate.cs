using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    [Table("ProjectUpdate")]
    public partial class ProjectUpdate
    {
        [Key]
        [Column("ID")]
        [StringLength(10)]
        public string Id { get; set; }
        [Column("projectID")]
        [StringLength(10)]
        public string ProjectId { get; set; }
        [Column("image")]
        public string Image { get; set; }
        [Column("title")]
        [StringLength(50)]
        public string Title { get; set; }
        [Column("description")]
        public string Description { get; set; }
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
        [InverseProperty("ProjectUpdates")]
        public virtual Project Project { get; set; }
    }
}
