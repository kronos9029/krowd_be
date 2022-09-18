using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs
{
    public class WalletTransactionDTO
    {
        public string id { get; set; }
        public string userId { get; set; }
        public string paymentId { get; set; }
        public string systemWalletId { get; set; }
        public string projectWalletId { get; set; }
        public string investorWalletId { get; set; }
        public float amount { get; set; }       
        public string description { get; set; }
        public string type { get; set; }
        public string fromWalletId { get; set; }
        public string toWalletId { get; set; }
        public float fee { get; set; }
        public string createDate { get; set; }
        public string createBy { get; set; }
        public string updateDate { get; set; }
        public string updateBy { get; set; }
        public bool isDeleted { get; set; }
    }
}
