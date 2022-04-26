﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    [Table("ProjectEntity")]
    public partial class ProjectEntity
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? ProjectId { get; set; }
        [StringLength(50)]
        public string Title { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        [StringLength(20)]
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
