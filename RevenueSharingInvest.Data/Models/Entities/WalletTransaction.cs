using System;
using System.Collections.Generic;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    public partial class WalletTransaction
    {
        public Guid Id { get; set; }
        public Guid? PaymentId { get; set; }
        public Guid? SystemWalletId { get; set; }
        public Guid? ProjectWalletId { get; set; }
        public Guid? InvestorWalletId { get; set; }
        public double? Amount { get; set; }
        public double? Fee { get; set; }
        public string Description { get; set; }
        public Guid? FromWalletId { get; set; }
        public Guid? ToWalletId { get; set; }
        public string Type { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }

        public virtual InvestorWallet InvestorWallet { get; set; }
        public virtual Payment Payment { get; set; }
        public virtual ProjectWallet ProjectWallet { get; set; }
        public virtual SystemWallet SystemWallet { get; set; }
    }
}
