using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    [Table("Area")]
    public partial class Area
    {
        public Area()
        {
            Projects = new HashSet<Project>();
        }

        [Key]
        public Guid Id { get; set; }
        [StringLength(50)]
        public string City { get; set; }
        [StringLength(50)]
        public string District { get; set; }
        [StringLength(50)]
        public string Ward { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        [StringLength(10)]
        public string CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        [StringLength(10)]
        public string UpdateBy { get; set; }
        public bool? IsDeleted { get; set; }

        [InverseProperty(nameof(Project.Area))]
        public virtual ICollection<Project> Projects { get; set; }
    }
}
