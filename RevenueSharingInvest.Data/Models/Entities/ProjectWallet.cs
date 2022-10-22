using System;
using System.Collections.Generic;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    public partial class ProjectWallet
    {
        public ProjectWallet()
        {
            WalletTransactions = new HashSet<WalletTransaction>();
        }

        public Guid Id { get; set; }
        public Guid WalletTypeId { get; set; }
        public Guid? ProjectId { get; set; }
        public Guid ProjectManagerId { get; set; }
        public double? Balance { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        public virtual User ProjectManager { get; set; }
        public virtual WalletType WalletType { get; set; }
        public virtual ICollection<WalletTransaction> WalletTransactions { get; set; }
    }
}
