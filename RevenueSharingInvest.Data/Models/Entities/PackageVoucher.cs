using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    [Table("PackageVoucher")]
    public partial class PackageVoucher
    {
        [Key]
        [Column("packageID")]
        [StringLength(10)]
        public string PackageId { get; set; }
        [Key]
        [Column("voucherID")]
        [StringLength(10)]
        public string VoucherId { get; set; }
        [Column("quantity")]
        public int? Quantity { get; set; }
        [Column("maxQuantity")]
        public int MaxQuantity { get; set; }

        [ForeignKey(nameof(PackageId))]
        [InverseProperty("PackageVouchers")]
        public virtual Package Package { get; set; }
        [ForeignKey(nameof(VoucherId))]
        [InverseProperty("PackageVouchers")]
        public virtual Voucher Voucher { get; set; }
    }
}
