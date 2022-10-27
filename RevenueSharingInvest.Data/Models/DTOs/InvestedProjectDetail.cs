using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs
{
    public class InvestedProjectDetail
    {
        public string ProjectImage { get; set; }
        public string ProjectName { get; set; }
        public string ProjectStatus { get; set; }
        public double ExpectedReturn { get; set; }
        public double ReturnedAmount { get; set; }
        public double DeptRemain { get; set; }
        public double InvestedAmount { get; set; }
        public int NumOfStage { get; set; }
        public int NumOfPayedStage { get; set; }
    }

    public class InvestedRecord
    {
        public string PackageName { get; set; }
        public int Quantity { get; set; }
        public double TotalPrice { get; set; }
        public DateTime CreateDate  { get; set; }
    }

    public class InvestedProjectDetailWithInvestment : InvestedProjectDetail
    {
        public double MustPaidDept { get; set; }
        public double ProfitableDebt { get; set; }
        public int PaidStage { get; set; }
        public DateTime LatestPayment { get; set; }
        public List<InvestedRecord> InvestmentRecords { get; set; }
    }
}
