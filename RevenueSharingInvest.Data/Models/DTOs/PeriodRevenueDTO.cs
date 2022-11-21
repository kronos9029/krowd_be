using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs
{
    public class PeriodRevenueDTO
    {
        public double actualAmount { get; set; }
        public double sharedAmount { get; set; }
        public double paidAmount { get; set; }
        public double pessimisticExpectedAmount { get; set; }
        public double normalExpectedAmount { get; set; }
        public double optimisticExpectedAmount { get; set; }
        public float pessimisticExpectedRatio { get; set; }
        public float normalExpectedRatio { get; set; }
        public float optimisticExpectedRatio { get; set; }
    }

    public class CreatePeriodRevenueDTO : PeriodRevenueDTO
    {
        public string projectId { get; set; }
        public string stageId { get; set; }
    }

    public class UpdatePeriodRevenueDTO : PeriodRevenueDTO
    {

    }

    public class GetPeriodRevenueDTO : PeriodRevenueDTO
    {
        public string id { get; set; }
        public string projectId { get; set; }
        public string stageId { get; set; }
        public string status { get; set; }
        public string createDate { get; set; }
        public string createBy { get; set; }
        public string updateDate { get; set; }
        public string updateBy { get; set; }
    }
}
