using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs
{
    public class InvestorTypeDTO
    {
        public string Id { get; set; }
        public string name { get; set; }
        public string description { get; set; }       
    }

    public class GetInvestorTypeDTO : InvestorTypeDTO
    {
        public string createDate { get; set; }
        public string createBy { get; set; }
        public string updateDate { get; set; }
        public string updateBy { get; set; }
        public bool isDeleted { get; set; }
    }

    public class UserInvestorTypeDTO : InvestorTypeDTO
    {

    }
}
