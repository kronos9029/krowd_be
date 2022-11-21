using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs
{
    public class ProjectDetailDTO
    {
        public string id { get; set; }
        public BusinessDTO business { get; set; }
        public UserDTO manager { get; set; }       
        public FieldDTO field { get; set; }
        public string areaId { get; set; }
        public string name { get; set; }
        public string image { get; set; }
        public string description { get; set; }
        public string address { get; set; }
        public float investmentTargetCapital { get; set; }
        public float investedCapital { get; set; }
        public float sharedRevenue { get; set; }
        public float multiplier { get; set; }
        public int duration { get; set; }
        public int numOfStage { get; set; }
        public float remainAmount { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public string businessLicense { get; set; }
        public string approvedDate { get; set; }
        public string approvedBy { get; set; }
        public int status { get; set; }
        public string createDate { get; set; }
        public string createBy { get; set; }
        public string updateDate { get; set; }
        public string updateBy { get; set; }
    }
}
