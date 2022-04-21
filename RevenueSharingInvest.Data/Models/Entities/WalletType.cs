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
            InvestorWallets = new HashSet<InvestorWallet>();
            ProjectWallets = new HashSet<ProjectWallet>();
            SystemWallets = new HashSet<SystemWallet>();
        }

        [Key]
        public Guid Id { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
        public string Description { get; set; }
        [StringLength(10)]
        public string Mode { get; set; }
        [StringLength(10)]
        public string Type { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
        public bool? IsDeleted { get; set; }

        [InverseProperty(nameof(InvestorWallet.WalletType))]
        public virtual ICollection<InvestorWallet> InvestorWallets { get; set; }
        [InverseProperty(nameof(ProjectWallet.WalletType))]
        public virtual ICollection<ProjectWallet> ProjectWallets { get; set; }
        [InverseProperty(nameof(SystemWallet.WalletType))]
        public virtual ICollection<SystemWallet> SystemWallets { get; set; }
    }
}
