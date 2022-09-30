using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs
{
    public class InvestorWalletDTO
    {
        public string id { get; set; }
        public string investorId { get; set; }
        public double balance { get; set; }        
        public string createDate { get; set; }
        public string createBy { get; set; }
        public string updateDate { get; set; }
        public string updateBy { get; set; }
    }

    public class MappedInvestorWalletDTO : InvestorWalletDTO
    {
        public string walletTypeId { get; set; }
    }

    public class GetInvestorWalletDTO : InvestorWalletDTO
    {
        public GetWalletTypeForWalletDTO walletType { get; set; }
    }
}
