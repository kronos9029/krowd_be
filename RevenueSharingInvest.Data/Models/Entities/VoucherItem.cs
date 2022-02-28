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
        [Column("investorID")]
        [StringLength(10)]
        public string InvestorId { get; set; }
        [Key]
        [Column("voucherId")]
        [StringLength(10)]
        public string VoucherId { get; set; }
        [Column("investmentID")]
        [StringLength(10)]
        public string InvestmentId { get; set; }
        [Column("issuedDate", TypeName = "datetime")]
        public DateTime? IssuedDate { get; set; }
        [Column("expireDate", TypeName = "datetime")]
        public DateTime? ExpireDate { get; set; }
        [Column("redeemDate", TypeName = "datetime")]
        public DateTime? RedeemDate { get; set; }
        [Column("availableDate", TypeName = "datetime")]
        public DateTime? AvailableDate { get; set; }

        [ForeignKey(nameof(InvestorId))]
        [InverseProperty("VoucherItems")]
        public virtual Investor Investor { get; set; }
        [ForeignKey(nameof(VoucherId))]
        [InverseProperty("VoucherItems")]
        public virtual Voucher Voucher { get; set; }
    }
}
