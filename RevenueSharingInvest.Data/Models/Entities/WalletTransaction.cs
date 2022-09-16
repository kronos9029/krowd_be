using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    [Table("WalletTransaction")]
    [Index(nameof(InvestorWalletId), Name = "IX_WalletTransaction_InvestorWalletId")]
    [Index(nameof(PaymentId), Name = "IX_WalletTransaction_PaymentId")]
    [Index(nameof(ProjectWalletId), Name = "IX_WalletTransaction_ProjectWalletId")]
    [Index(nameof(SystemWalletId), Name = "IX_WalletTransaction_SystemWalletId")]
    [Index(nameof(UserId), Name = "IX_WalletTransaction_User")]
    public partial class WalletTransaction
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid? PaymentId { get; set; }
        public Guid? SystemWalletId { get; set; }
        public Guid? ProjectWalletId { get; set; }
        public Guid? InvestorWalletId { get; set; }
        public double? Amount { get; set; }
        public double? Fee { get; set; }
        public string Description { get; set; }
        public Guid? FromWalletId { get; set; }
        public Guid? ToWalletId { get; set; }
        [StringLength(20)]
        public string Type { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
        public bool? IsDeleted { get; set; }

        [ForeignKey(nameof(InvestorWalletId))]
        [InverseProperty("WalletTransactions")]
        public virtual InvestorWallet InvestorWallet { get; set; }
        [ForeignKey(nameof(PaymentId))]
        [InverseProperty("WalletTransactions")]
        public virtual Payment Payment { get; set; }
        [ForeignKey(nameof(ProjectWalletId))]
        [InverseProperty("WalletTransactions")]
        public virtual ProjectWallet ProjectWallet { get; set; }
        [ForeignKey(nameof(SystemWalletId))]
        [InverseProperty("WalletTransactions")]
        public virtual SystemWallet SystemWallet { get; set; }
        [ForeignKey(nameof(UserId))]
        [InverseProperty("WalletTransactions")]
        public virtual User User { get; set; }
    }
}
