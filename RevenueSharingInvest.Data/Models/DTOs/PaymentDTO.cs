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
        public float amount { get; set; }
        public string description { get; set; }
        public string type { get; set; }
        public string fromId { get; set; }
        public string toId { get; set; }
        public string createDate { get; set; }
        public string createBy { get; set; }
        public string updateDate { get; set; }
        public string updateBy { get; set; }
        public bool isDeleted { get; set; }
    }
}
