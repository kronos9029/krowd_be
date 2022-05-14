using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs
{
    public class InvestmentDTO
    {
        public string investorId { get; set; }
        public string projectId { get; set; }
        public string packageId { get; set; }
        public int quantity { get; set; }
        public float totalPrice { get; set; }
        public DateTime lastPayment { get; set; }
        public DateTime createDate { get; set; }
        public string createBy { get; set; }
        public DateTime updateDate { get; set; }
        public string updateBy { get; set; }
        public bool isDeleted { get; set; }
    }
}
