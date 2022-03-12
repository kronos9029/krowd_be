using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    [Table("InvestorWallet")]
    public partial class InvestorWallet
    {
        public InvestorWallet()
        {
            WalletTransactions = new HashSet<WalletTransaction>();
        }

        [Key]
        public Guid Id { get; set; }
        public Guid? InvestorId { get; set; }
        public double? Balance { get; set; }
        public Guid? WalletTypeId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreateDate { get; set; }
        [StringLength(10)]
        public string CreateBy { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdateDate { get; set; }
        [StringLength(10)]
        public string UpDateBy { get; set; }
        public bool? IsDeleted { get; set; }

        [ForeignKey(nameof(InvestorId))]
        [InverseProperty("InvestorWallets")]
        public virtual Investor Investor { get; set; }
        [ForeignKey(nameof(WalletTypeId))]
        [InverseProperty("InvestorWallets")]
        public virtual WalletType WalletType { get; set; }
        [InverseProperty(nameof(WalletTransaction.InvestorWallet))]
        public virtual ICollection<WalletTransaction> WalletTransactions { get; set; }
    }
}
