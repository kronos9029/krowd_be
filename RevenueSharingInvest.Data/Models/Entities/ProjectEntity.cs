using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using RevenueSharingInvest.Data.Models.Constants;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    [Table("ProjectEntity")]
    public partial class ProjectEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }
        public Guid? ProjectId { get; set; }
        [StringLength(50)]
        public string Title { get; set; }        
        [StringLength(int.MaxValue)]
        public string Content { get; set; }
        [StringLength(int.MaxValue)]
        public string Description { get; set; }
        [StringLength(int.MaxValue)]
        public string Link { get; set; }
        public string Type { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
        public bool? IsDeleted { get; set; }

        [ForeignKey(nameof(ProjectId))]
        [InverseProperty("ProjectEntities")]
        public virtual Project Project { get; set; }
    }
}
