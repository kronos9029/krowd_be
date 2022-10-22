using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs
{
    public class GetWithdrawRequestDTO
    {
        public string Id { get; set; }
        public string BankName { get; set; }
        public string AccountName { get; set; }
        public string BankAccount { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public string Status { get; set; }
        public string RefusalReason { get; set; }
        public string CreateDate { get; set; }
        public string CreateBy { get; set; }
        public string UpdateDate { get; set; }
        public string UpdateBy { get; set; }
    }

    public class InvestorWithdrawRequest
    {
        public string FromWalletId { get; set; }
        public string ToWalletId { get; set; }
        public string BankName { get; set; }
        public string AccountName { get; set; }
        public string BankAccount { get; set; }
        public double Amount { get; set; }
        public string WalletId { get; set; }
    }

    public class UpdateWithdrawRequest
    {
        public string requestId { get; set; }
        public string description { get; set; }
        public string refusalReason { get; set; }
    }
}
