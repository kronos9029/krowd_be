using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs
{
    public class StageDTO
    {
        public string name { get; set; }          
        //public float percents { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
    }

    public class CreateStageDTO : StageDTO
    {
        public string projectId { get; set; }
    }

    public class UpdateStageDTO : StageDTO
    {
        public string description { get; set; }
        public float pessimisticExpectedAmount { get; set; }
        public float normalExpectedAmount { get; set; }
        public float optimisticExpectedAmount { get; set; }
        public float pessimisticExpectedRatio { get; set; }
        public float normalExpectedRatio { get; set; }
        public float optimisticExpectedRatio { get; set; }
    }

    public class GetStageDTO : StageDTO
    {
        public string id { get; set; }
        public string projectId { get; set; }
        public string status { get; set; }
        public string createDate { get; set; }
        public string createBy { get; set; }
        public string updateDate { get; set; }
        public string updateBy { get; set; }
        public bool isDeleted { get; set; }
        public float pessimisticExpectedAmount { get; set; }
        public float normalExpectedAmount { get; set; }
        public float optimisticExpectedAmount { get; set; }
        public float pessimisticExpectedRatio { get; set; }
        public float normalExpectedRatio { get; set; }
        public float optimisticExpectedRatio { get; set; }
    }
}
