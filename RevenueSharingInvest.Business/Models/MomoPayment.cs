using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Models
{

    public class MomoPaymentResult
    {
        public string partnerCode { get; set; }
        public string requestId { get; set; }
        public string orderId { get; set; }
        public long amount { get; set; }
        public long responseTime { get; set; }
        public string message { get; set; }
        public int resultCode { get; set; }
        public string PayUrl { get; set; }
        public string QrCodeUrl { get; set; }
        public string Deeplink { get; set; }
        public string partnerClientId { get; set; }
    }

    public class MomoPaymentRequest
    {
        public string partnerCode { get; set; }
        public string accessKey { get; set; }
        public string requestId { get; set; }
        public string amount { get; set; }
        public string orderId { get; set; }
        public string orderInfo { get; set; }
        public string orderType { get; set; }
        public string transId { get; set; }
        public string message { get; set; }
        public string localMessage { get; set; }
        public string responseTime { get; set; }
        public string errorCode { get; set; }
        public string payType { get; set; }
        public string extraData { get; set; }
        public string signature { get; set; }
    }

}
