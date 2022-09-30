using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs
{
    public class PaymentDTO
    {
        public string id { get; set; }
        public string periodRevenueId { get; set; }
        public string investmentId { get; set; }       
        public double amount { get; set; }
        public string description { get; set; }
        public string type { get; set; }
        public string fromId { get; set; }
        public string toId { get; set; }
        public string createDate { get; set; }
        public string createBy { get; set; }
        public string status { get; set; }
    }

    public class InvestmentPaymentDTO : PaymentDTO
    {
        public string projectId { get; set; }
        public string projectName { get; set; }
        public string packageId { get; set; }
        public string packageName { get; set; }
        public int investedQuantity { get; set; }
        public string fromWalletName { get; set; }
        public string fee { get; set; }
    }
    public class PeriodRevenuePaymentDTO : PaymentDTO
    {
        public string projectId { get; set; }
        public string projectName { get; set; }
        public string stageId { get; set; }
        public string stageName { get; set; }
        public int investedQuantity { get; set; }
        public string fromWalletName { get; set; }
        public string fee { get; set; }
    }
}
