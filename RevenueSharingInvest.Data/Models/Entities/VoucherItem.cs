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
        public Guid Id { get; set; }
        public Guid VoucherId { get; set; }
        public Guid InvestmentId { get; set; }
        public long? IssuedDate { get; set; }
        public long? ExpireDate { get; set; }
        public long? RedeemDate { get; set; }
        public long? AvailableDate { get; set; }
        public long? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public long? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
        public bool? IsDeleted { get; set; }

        [ForeignKey(nameof(InvestmentId))]
        [InverseProperty("VoucherItems")]
        public virtual Investment Investment { get; set; }
        [ForeignKey(nameof(VoucherId))]
        [InverseProperty("VoucherItems")]
        public virtual Voucher Voucher { get; set; }
    }
}
