using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

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
        public string Type { get; set; }
        ///MOMO
        public string PartnerCode { get; set; }
        public string OrderId { get; set; }
        public string RequestId { get; set; }
        public string PartnerUserId { get; set; }
        public Guid? PartnerClientId { get; set; }
        public long TransId { get; set; }
        public long Amount { get; set; }
        public string OrderInfo { get; set; }
        public string OrderType { get; set; }
        public string CallbackToken { get; set; }
        public int ResultCode { get; set; }
        public string Message { get; set; }
        public string PayType { get; set; }
        public long ResponseTime { get; set; }
        public string ExtraData { get; set; }
        public string Signature { get; set; }
        public DateTime CreateDate { get; set; }


        [ForeignKey(nameof(FromUserId))]
        [InverseProperty(nameof(User.AccountTransactionFromUsers))]
        public virtual User FromUser { get; set; }
        [ForeignKey(nameof(ToUserId))]
        [InverseProperty(nameof(User.AccountTransactionToUsers))]
        public virtual User ToUser { get; set; }
    }
}
