﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    [Table("AccountTransaction")]
    [Index(nameof(FromUserId), Name = "IX_AccountTransaction_FromUserId")]
    [Index(nameof(ToUserId), Name = "IX_AccountTransaction_ToUserId")]
    public partial class AccountTransaction
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? FromUserId { get; set; }
        public Guid? ToUserId { get; set; }
        public string PartnerUserId { get; set; }
        public Guid? PartnerClientId { get; set; }
        public long Amount { get; set; }
        public string OrderType { get; set; }
        public string Message { get; set; }
        public string OrderId { get; set; }
        public string PartnerCode { get; set; }
        public string PayType { get; set; }
        public string Signature { get; set; }
        public string RequestId { get; set; }
        public long ResponseTime { get; set; }
        public int ResultCode { get; set; }
        public string CallbackToken { get; set; }
        public string ExtraData { get; set; }
        public string OrderInfo { get; set; }
        public long TransId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public string Type { get; set; }
        public Guid? WithdrawRequestId { get; set; }

        [ForeignKey(nameof(FromUserId))]
        [InverseProperty(nameof(User.AccountTransactionFromUsers))]
        public virtual User FromUser { get; set; }
        [ForeignKey(nameof(ToUserId))]
        [InverseProperty(nameof(User.AccountTransactionToUsers))]
        public virtual User ToUser { get; set; }
        [ForeignKey(nameof(WithdrawRequestId))]
        [InverseProperty("AccountTransactions")]
        public virtual WithdrawRequest WithdrawRequest { get; set; }
    }
}
