using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    [Table("Voucher")]
    public partial class Voucher
    {
        public Voucher()
        {
            PackageVouchers = new HashSet<PackageVoucher>();
            VoucherItems = new HashSet<VoucherItem>();
        }

        [Key]
        public Guid Id { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
        [StringLength(10)]
        public string Code { get; set; }
        public Guid? ProjectId { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public int? Quantity { get; set; }
        [StringLength(20)]
        public string Status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? StartDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? EndDate { get; set; }
        [StringLength(10)]
        public string CreateDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateBy { get; set; }
        [StringLength(10)]
        public string UpdateDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateBy { get; set; }
        public bool? IsDeleted { get; set; }

        [ForeignKey(nameof(ProjectId))]
        [InverseProperty("Vouchers")]
        public virtual Project Project { get; set; }
        [InverseProperty(nameof(PackageVoucher.Voucher))]
        public virtual ICollection<PackageVoucher> PackageVouchers { get; set; }
        [InverseProperty(nameof(VoucherItem.Voucher))]
        public virtual ICollection<VoucherItem> VoucherItems { get; set; }
    }
}
