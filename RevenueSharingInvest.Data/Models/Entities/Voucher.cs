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
        [Column("ID")]
        [StringLength(10)]
        public string Id { get; set; }
        [Column("name")]
        [StringLength(50)]
        public string Name { get; set; }
        [Column("code")]
        [StringLength(10)]
        public string Code { get; set; }
        [Column("projectID")]
        [StringLength(10)]
        public string ProjectId { get; set; }
        [Column("description")]
        public string Description { get; set; }
        [Column("image")]
        public string Image { get; set; }
        [Column("quantity")]
        public int? Quantity { get; set; }
        [Column("status")]
        [StringLength(20)]
        public string Status { get; set; }
        [Column("startDate", TypeName = "datetime")]
        public DateTime? StartDate { get; set; }
        [Column("endDate", TypeName = "datetime")]
        public DateTime? EndDate { get; set; }
        [Column("createDate")]
        [StringLength(10)]
        public string CreateDate { get; set; }
        [Column("createBy", TypeName = "datetime")]
        public DateTime? CreateBy { get; set; }
        [Column("updateDate")]
        [StringLength(10)]
        public string UpdateDate { get; set; }
        [Column("updateBy", TypeName = "datetime")]
        public DateTime? UpdateBy { get; set; }
        [Column("isDeleted")]
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
