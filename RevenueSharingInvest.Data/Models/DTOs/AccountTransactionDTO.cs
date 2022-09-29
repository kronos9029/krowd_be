using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Data.Models.DTOs
{
    public class AccountTransactionDTO
    {
        public string id { get; set; }
        public string fromUserId { get; set; }
        public string partnerClientId { get; set; }
        public string amount { get; set; }
        public string orderType { get; set; }
        public string message { get; set; }
        public string orderId { get; set; }
        public string partnerCode { get; set; }
        public string payType { get; set; }
        public string signature { get; set; }
        public string requestId { get; set; }
        public string responsetime { get; set; }
        public string resultCode { get; set; }
        public string extraData { get; set; }
        public string orderInfo { get; set; }
        public string transId { get; set; }
        public string createDate { get; set; }
        public string Type { get; set; }
    }
}
