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
            InvestorLocations = new HashSet<InvestorLocation>();
            Projects = new HashSet<Project>();
        }

        [Key]
        [Column("ID")]
        [StringLength(10)]
        public string Id { get; set; }
        [Column("city")]
        [StringLength(50)]
        public string City { get; set; }
        [Column("district")]
        [StringLength(50)]
        public string District { get; set; }
        [Column("ward")]
        [StringLength(50)]
        public string Ward { get; set; }
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

        [InverseProperty(nameof(InvestorLocation.Area))]
        public virtual ICollection<InvestorLocation> InvestorLocations { get; set; }
        [InverseProperty(nameof(Project.Area))]
        public virtual ICollection<Project> Projects { get; set; }
    }
}
