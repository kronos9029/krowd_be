using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs
{
    public class VoucherDTO
    {
        public string id { get; set; }
        public string projectId { get; set; }
        public string name { get; set; }
        public string code { get; set; }       
        public string description { get; set; }
        public string image { get; set; }
        public int quantity { get; set; }
        public string status { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public string createDate { get; set; }
        public string createBy { get; set; }
        public string updateDate { get; set; }
        public string updateBy { get; set; }
        public bool isDeleted { get; set; }
    }
}
