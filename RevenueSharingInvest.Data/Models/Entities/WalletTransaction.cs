using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    [Table("WalletTransaction")]
    public partial class WalletTransaction
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? PaymentId { get; set; }
        public Guid? SystemWalletId { get; set; }
        public Guid? ProjectWalletId { get; set; }
        public Guid? InvestorWalletId { get; set; }
        public double? Amount { get; set; }
        public string Description { get; set; }
        [StringLength(20)]
        public string Type { get; set; }
        [StringLength(10)]
        public string FromWalletId { get; set; }
        [StringLength(10)]
        public string ToWalletId { get; set; }
        public double? Fee { get; set; }
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
    }
}
