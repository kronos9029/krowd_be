using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    [Table("WalletType")]
    public partial class WalletType
    {
        public WalletType()
        {
            BusinessWallets = new HashSet<BusinessWallet>();
            InvestorWallets = new HashSet<InvestorWallet>();
            SystemWallets = new HashSet<SystemWallet>();
        }

        [Key]
        [Column("ID")]
        [StringLength(10)]
        public string Id { get; set; }
        [Column("name")]
        [StringLength(50)]
        public string Name { get; set; }
        [Column("description")]
        public string Description { get; set; }
        [Column("isDeleted")]
        public bool? IsDeleted { get; set; }

        [InverseProperty(nameof(BusinessWallet.WalletType))]
        public virtual ICollection<BusinessWallet> BusinessWallets { get; set; }
        [InverseProperty(nameof(InvestorWallet.WalletType))]
        public virtual ICollection<InvestorWallet> InvestorWallets { get; set; }
        [InverseProperty(nameof(SystemWallet.WalletType))]
        public virtual ICollection<SystemWallet> SystemWallets { get; set; }
    }
}
