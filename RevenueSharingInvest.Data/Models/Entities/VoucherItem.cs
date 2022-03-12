using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    [Table("VoucherItem")]
    public partial class VoucherItem
    {
        [Key]
        public Guid InvestorId { get; set; }
        [Key]
        public Guid VoucherId { get; set; }
        [StringLength(10)]
        public string InvestmentId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? IssuedDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ExpireDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? RedeemDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? AvailableDate { get; set; }

        [ForeignKey(nameof(InvestorId))]
        [InverseProperty("VoucherItems")]
        public virtual Investor Investor { get; set; }
        [ForeignKey(nameof(VoucherId))]
        [InverseProperty("VoucherItems")]
        public virtual Voucher Voucher { get; set; }
    }
}
