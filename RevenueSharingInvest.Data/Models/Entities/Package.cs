using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    [Table("Package")]
    [Index(nameof(ProjectId), Name = "IX_Package_ProjectId")]
    public partial class Package
    {
        public Package()
        {
            Investments = new HashSet<Investment>();
            PackageVouchers = new HashSet<PackageVoucher>();
        }

        [Key]
        public Guid Id { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
        public Guid ProjectId { get; set; }
        public double Price { get; set; }
        public string Image { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
        public int RemainingQuantity { get; set; }
        [Required]
        [StringLength(12)]
        public string Status { get; set; }

        [ForeignKey(nameof(ProjectId))]
        [InverseProperty("Packages")]
        public virtual Project Project { get; set; }
        [InverseProperty(nameof(Investment.Package))]
        public virtual ICollection<Investment> Investments { get; set; }
        [InverseProperty(nameof(PackageVoucher.Package))]
        public virtual ICollection<PackageVoucher> PackageVouchers { get; set; }
    }
}
