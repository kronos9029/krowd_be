using System;
using System.Collections.Generic;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    public partial class InvestorWallet
    {
        public InvestorWallet()
        {
            WalletTransactions = new HashSet<WalletTransaction>();
        }

        public Guid Id { get; set; }
        public Guid? InvestorId { get; set; }
        public double? Balance { get; set; }
        public Guid? WalletTypeId { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        public virtual Investor Investor { get; set; }
        public virtual WalletType WalletType { get; set; }
        public virtual ICollection<WalletTransaction> WalletTransactions { get; set; }
    }
}
