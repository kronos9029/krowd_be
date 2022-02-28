using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    [Table("Package")]
    public partial class Package
    {
        public Package()
        {
            Investments = new HashSet<Investment>();
            PackageVouchers = new HashSet<PackageVoucher>();
        }

        [Key]
        [Column("ID")]
        [StringLength(10)]
        public string Id { get; set; }
        [Column("name")]
        [StringLength(50)]
        public string Name { get; set; }
        [Column("projectID")]
        [StringLength(10)]
        public string ProjectId { get; set; }
        [Column("price")]
        public double? Price { get; set; }
        [Column("image")]
        public string Image { get; set; }
        [Column("quantity")]
        public int? Quantity { get; set; }
        [Column("description")]
        public string Description { get; set; }
        [Column("minForPurchasing")]
        public int? MinForPurchasing { get; set; }
        [Column("maxForPurchasing")]
        public int? MaxForPurchasing { get; set; }
        [Column("openDate", TypeName = "datetime")]
        public DateTime? OpenDate { get; set; }
        [Column("closeDate", TypeName = "datetime")]
        public DateTime? CloseDate { get; set; }
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
        [Column("approvedDate", TypeName = "datetime")]
        public DateTime? ApprovedDate { get; set; }
        [Column("approvedBy")]
        [StringLength(10)]
        public string ApprovedBy { get; set; }
        [Column("isDeleted")]
        public bool? IsDeleted { get; set; }

        [ForeignKey(nameof(ProjectId))]
        [InverseProperty("Packages")]
        public virtual Project Project { get; set; }
        [InverseProperty(nameof(Investment.Package))]
        public virtual ICollection<Investment> Investments { get; set; }
        [InverseProperty(nameof(PackageVoucher.Package))]
        public virtual ICollection<PackageVoucher> PackageVouchers { get; set; }
    }
}
