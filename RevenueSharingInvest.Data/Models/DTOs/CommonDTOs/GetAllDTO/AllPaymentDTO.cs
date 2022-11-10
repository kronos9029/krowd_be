using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs.CommonDTOs
{
    public class AllPaymentDTO
    {
        public int numOfPayment { get; set; }
        public List<InvestmentPaymentDTO> listOfInvestmentPayment { get; set; }
        public List<PeriodRevenuePaymentDTO> listOfPeriodRevenuePayment { get; set; }
    }
}
