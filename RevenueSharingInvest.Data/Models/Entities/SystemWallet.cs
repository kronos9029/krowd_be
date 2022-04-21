using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    [Table("SystemWallet")]
    public partial class SystemWallet
    {
        public SystemWallet()
        {
            WalletTransactions = new HashSet<WalletTransaction>();
        }

        [Key]
        public Guid Id { get; set; }
        public double? Balance { get; set; }
        public Guid? WalletTypeId { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
        public bool? IsDeleted { get; set; }

        [ForeignKey(nameof(WalletTypeId))]
        [InverseProperty("SystemWallets")]
        public virtual WalletType WalletType { get; set; }
        [InverseProperty(nameof(WalletTransaction.SystemWallet))]
        public virtual ICollection<WalletTransaction> WalletTransactions { get; set; }
    }
}
