using System;
using System.Collections.Generic;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    public partial class Investor
    {
        public Investor()
        {
            InvestorWallets = new HashSet<InvestorWallet>();
        }

        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public Guid InvestorTypeId { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }
        public string Status { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<InvestorWallet> InvestorWallets { get; set; }
    }
}
