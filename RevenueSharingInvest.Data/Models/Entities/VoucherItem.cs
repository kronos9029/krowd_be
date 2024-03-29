﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    [Table("VoucherItem")]
    [Index(nameof(InvestmentId), Name = "IX_VoucherItem_InvestmentId")]
    [Index(nameof(VoucherId), Name = "IX_VoucherItem_VoucherId")]
    public partial class VoucherItem
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? VoucherId { get; set; }
        public Guid? InvestmentId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? IssuedDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ExpireDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? RedeemDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? AvailableDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        [ForeignKey(nameof(InvestmentId))]
        [InverseProperty("VoucherItems")]
        public virtual Investment Investment { get; set; }
        [ForeignKey(nameof(VoucherId))]
        [InverseProperty("VoucherItems")]
        public virtual Voucher Voucher { get; set; }
    }
}
