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
        public Guid PackageId { get; set; }
        [Key]
        public Guid VoucherId { get; set; }
        public int? Quantity { get; set; }
        public int MaxQuantity { get; set; }

        [ForeignKey(nameof(PackageId))]
        [InverseProperty("PackageVouchers")]
        public virtual Package Package { get; set; }
        [ForeignKey(nameof(VoucherId))]
        [InverseProperty("PackageVouchers")]
        public virtual Voucher Voucher { get; set; }
    }
}
