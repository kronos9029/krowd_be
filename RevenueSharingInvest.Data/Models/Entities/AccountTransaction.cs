using System;
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
        public string Description { get; set; }
        [StringLength(10)]
        public string Status { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
        public bool? IsDeleted { get; set; }
        ///MOMO
        public string PartnerCode { get; set; }
        public string OrderId { get; set; }
        public string RequestId { get; set; }
        public long Amount { get; set; }
        public long ResponseTime { get; set; }
        public string Message { get; set; }
        public int ResultCode { get; set; }
        public string PayUrl { get; set; }
        public string Deeplink { get; set; }
        public string QrCodeUrl { get; set; }

        [ForeignKey(nameof(FromUserId))]
        [InverseProperty(nameof(User.AccountTransactionFromUsers))]
        public virtual User FromUser { get; set; }
        [ForeignKey(nameof(ToUserId))]
        [InverseProperty(nameof(User.AccountTransactionToUsers))]
        public virtual User ToUser { get; set; }
    }
}
