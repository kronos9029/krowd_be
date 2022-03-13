using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs
{
    public class PeriodRevenueDTO
    {
        public string id { get; set; }
        public string projectId { get; set; }
        public string stageId { get; set; }
        public float actualAmount { get; set; }
        public float pessimisticExpectedAmount { get; set; }
        public float normalExpectedAmount { get; set; }
        public float optimisticExpectedAmount { get; set; }
        public float pessimisticExpectedRatio { get; set; }
        public float normalExpectedRatio { get; set; }
        public float optimisticExpectedRatio { get; set; }
        public string status { get; set; }
        public DateTime createDate { get; set; }
        public string createBy { get; set; }
        public DateTime updateDate { get; set; }
        public string updateBy { get; set; }
        public bool isDeleted { get; set; }

    }
}
