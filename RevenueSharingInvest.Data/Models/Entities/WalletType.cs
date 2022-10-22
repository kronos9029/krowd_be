using System;
using System.Collections.Generic;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    public partial class WalletType
    {
        public WalletType()
        {
            InvestorWallets = new HashSet<InvestorWallet>();
            ProjectWallets = new HashSet<ProjectWallet>();
            SystemWallets = new HashSet<SystemWallet>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string Mode { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? UpdateBy { get; set; }
        public DateTime? UpdateDate { get; set; }

        public virtual ICollection<InvestorWallet> InvestorWallets { get; set; }
        public virtual ICollection<ProjectWallet> ProjectWallets { get; set; }
        public virtual ICollection<SystemWallet> SystemWallets { get; set; }
    }
}
