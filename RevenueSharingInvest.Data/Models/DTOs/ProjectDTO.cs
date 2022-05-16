using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs
{
    public class ProjectDTO
    {
        public string id { get; set; }
        public string managerId { get; set; }
        public string businessId { get; set; }
        public string name { get; set; }
        public string image { get; set; }
        public string description { get; set; }
        public string category { get; set; }
        public string address { get; set; }
        public string areaId { get; set; }
        public float investmentTargetCapital { get; set; }
        public float investedCapital { get; set; }
        public float sharedRevenue { get; set; }
        public float multiplier { get; set; }
        public int duration { get; set; }
        public int numOfStage { get; set; }
        public float remainAmount { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public string businessLicense { get; set; }
        public string status { get; set; }
        public DateTime createDate { get; set; }
        public string createBy { get; set; }
        public DateTime updateDate { get; set; }
        public string updateBy { get; set; }
        public DateTime approvedDate { get; set; }
        public string approvedBy { get; set; }
        public bool isDeleted { get; set; }
    }
}
