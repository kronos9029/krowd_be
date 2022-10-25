using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs
{
    public class InvestedProjectDetail
    {
        public string ProjectName { get; set; }
        public string ProjectStatus { get; set; }
        public double ExpectedRevenue { get; set; }
        public int NumOfStage { get; set; }
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
        public List<InvestedRecord> investmentRecords { get; set; }
    }
}
