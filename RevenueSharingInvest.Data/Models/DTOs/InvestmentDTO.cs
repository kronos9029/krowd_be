using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs
{
    public class InvestmentDTO
    {        
        public string projectId { get; set; }
        public string packageId { get; set; }
        public int quantity { get; set; }
    }

    public class CreateInvestmentDTO : InvestmentDTO
    {

    }

    public class GetInvestmentDTO : InvestmentDTO
    {
        public string id { get; set; }
        public string investorId { get; set; }
        public double totalPrice { get; set; }
        public string status { get; set; }
        public string createDate { get; set; }
        public string createBy { get; set; }
        public string updateDate { get; set; }
        public string updateBy { get; set; }
    }
}
