using System;
using System.Collections.Generic;

#nullable disable

namespace RevenueSharingInvest.Data.Models.Entities
{
    public partial class WithdrawRequest
    {
        public WithdrawRequest()
        {
            AccountTransactions = new HashSet<AccountTransaction>();
        }

        public Guid Id { get; set; }
        public string BankName { get; set; }
        public string AccountName { get; set; }
        public string BankAccount { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public string Status { get; set; }
        public string RefusalReason { get; set; }
        public DateTime? CreateDate { get; set; }
        public Guid? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public Guid? UpdateBy { get; set; }

        public virtual User CreateByNavigation { get; set; }
        public virtual User UpdateByNavigation { get; set; }
        public virtual ICollection<AccountTransaction> AccountTransactions { get; set; }
    }
}
