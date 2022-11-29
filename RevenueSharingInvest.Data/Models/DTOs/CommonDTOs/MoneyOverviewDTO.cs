using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs.CommonDTOs
{
    public class MoneyOverviewDTO
    {
        public double totalDepositedAmount { get; set; }
        public double totalWithdrawedAmount { get; set; }
        public double totalInvestedAmount { get; set; }      
        public double totalReceivedAmount { get; set; }
        public int numOfInvestedProject { get; set; }
        public int numOfCallingForInvestmentInvestedProject { get; set; }
        public int numOfActiveInvestedProject { get; set; }
        public int numOfInvestment { get; set; }
        public int numOfSuccessInvestment { get; set; }
        public int numOfFailedInvestment { get; set; }
        public int numOfCanceledInvestment { get; set; }
    }
}
