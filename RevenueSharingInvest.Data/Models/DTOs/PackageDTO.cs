using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs
{
    public class PackageDTO
    {
        public string id { get; set; }
        public string name { get; set; }
        public string projectId { get; set; }
        public float price { get; set; }
        public string image { get; set; }
        public int quantity { get; set; }
        public string description { get; set; }
        public int minForPurchasing { get; set; }
        public int maxForPurchasing { get; set; }
        public DateTime openDate { get; set; }
        public DateTime closeDate { get; set; }
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
