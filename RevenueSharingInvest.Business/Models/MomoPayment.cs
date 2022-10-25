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
        public string orderId { get; set; }
        public string requestId { get; set; }
        public long amount { get; set; }
        public string orderInfo { get; set; }
        public string partnerUserId { get; set; }
        public string orderType { get; set; }
        public string partnerClientId { get; set; }
        public string callbackToken { get; set; }
        public long transId { get; set; }
        public int resultCode { get; set; }
        public string message { get; set; }
        public string payType { get; set; }
        public long responseTime { get; set; }
        public string extraData { get; set; }
        public string signature { get; set; }


    }
    public class MomoPaymentResponse
    {
        public string partnerCode { get; set; }
        public string requestId { get; set; }
        public string orderId { get; set; }
        public long amount { get; set; }
        public long responseTime { get; set; }
        public string message { get; set; }
        public int resultCode { get; set; }
        public string payUrl { get; set; }
        public string deeplink { get; set; }
        public string qrCodeUrl { get; set; }
        public string deeplinkMiniApp { get; set; }
        public string partnerClientId { get; set; }
    }

    public class MomoPaymentRequest
    {
        public string partnerClientId { get; set; }
        public string email { get; set; }
        public string partnerCode { get; set; }
        public string requestId { get; set; }
        public long amount { get; set; }
        public string orderId { get; set; }
        public string orderInfo { get; set; }
        public long orderGroupId { get; set; }
        public string redirectUrl { get; set; }
        public string ipnUrl { get; set; }
        public string requestType { get; set; }
        public string extraData { get; set; }
        public string lang { get; set; }
        public string signature { get; set; }
        public string role { get; set; }

    }

    public class UserInfo
    {
        public string name { get; set; }
        public string phoneNumber { get; set; }
        public string email { get; set; }
    }

    public class QueryRequest
    {
        public string partnerCode { get; set; }
        public string requestId { get; set; }
        public string orderId { get; set; }
        public string lang { get; set; }
        public string signature { get; set; }

    }

    public class QueryResponse
    {
        public string partnerCode { get; set; }
        public string requestId { get; set; }
        public string orderId { get; set; }
        public string extraData { get; set; }
        public long amount { get; set; }
        public long transId { get;set; }
        public string payType { get; set; }
        public int resultCode { get; set; }
        public JsonArray refundsTrans { get; set; }
        public long responseTime { get; set; }
        public string message { get; set; }
    }

    public class ConfirmRequest
    {
        public string partnerCode { get; set; }
        public string requestId { get; set; }
        public string orderId { get; set; }
        public string requestType { get; set; }
        public long amount { get; set; }
        public string lang { get; set; }
        public string description { get; set; }
        public string signature { get; set; }

    }

    public class ConfirmResponse
    {
        public string partnerCode { get; set; }
        public string requestId { get; set; }
        public string orderId { get; set; }
        public long amount { get; set; }
        public long transId { get; set; }
        public int resultCode { get; set; }
        public string message { get; set; }
        public string requestType { get; set; }
        public long responseTime { get; set; }

    }

}
