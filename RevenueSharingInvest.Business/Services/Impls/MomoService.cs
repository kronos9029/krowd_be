using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using RevenueSharingInvest.Business.Helpers;
using RevenueSharingInvest.Business.Models;
using RevenueSharingInvest.Business.Services.Extensions;
using RevenueSharingInvest.Business.Services.Extensions.Momo;
using RevenueSharingInvest.Data.Repositories.IRepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevenueSharingInvest.Business.Services.Impls
{
    public class MomoService : IMomoService
    {
        private readonly MomoSettings _momoSettings;
        public MomoService(IOptions<MomoSettings> momoSettings)
        {
            _momoSettings = momoSettings.Value;
        }

        public async Task<string> RequestPaymentWeb(MomoPaymentRequest request)
        {
            //request params need to request to MoMo system
            string endpoint = _momoSettings.ApiEndpoint;
            string partnerCode = _momoSettings.PartnerCode;
            string accessKey = _momoSettings.AccessKey;
            string serectkey = _momoSettings.SecretKey;
            string orderInfo = "test";
            string returnUrl = _momoSettings.ReturnUrl;
            string notifyurl = _momoSettings.NotifyUrl; //lưu ý: notifyurl không được sử dụng localhost, có thể sử dụng ngrok để public localhost trong quá trình test

            string amount = "2000000";
            string orderid = Guid.NewGuid().ToString(); //mã đơn hàng
            string requestId = Guid.NewGuid().ToString();
            string extraData = "topup";
            string requestType = "captureWallet";

            //Before sign HMAC SHA256 signature
            string rawHash = "accessKey=" + accessKey +
                "&amount=" + amount +
                "&extraData=" + extraData +
                "&ipnUrl=" + notifyurl +
                "&orderId=" + orderid +
                "&orderInfo=" + orderInfo +
                "&partnerCode=" + partnerCode +
                "&redirectUrl=" + returnUrl +
                "&requestId=" + requestId +
                "&requestType=" + requestType
                ;

            MomoSecurity crypto = new MomoSecurity();
            //sign signature SHA256
            string signature = crypto.signSHA256(rawHash, serectkey);


            //build body json request
            JObject message = new JObject
            {
                { "partnerCode", partnerCode },
                { "partnerName", "Revenue Sharing Invest" },
                { "storeId", "MomoTestStore" },
                { "requestId", requestId },
                { "amount", amount },
                { "orderId", orderid },
                { "orderInfo", orderInfo },
                { "redirectUrl", returnUrl },
                { "ipnUrl", notifyurl },
                { "lang", "vi" },
                { "extraData", extraData },
                { "requestType", requestType },
                { "signature", signature }

            };

            string responseFromMomo = PaymentRequest.sendPaymentRequest(endpoint, message.ToString());

            JObject jmessage = JObject.Parse(responseFromMomo);

            return jmessage.GetValue("payUrl").ToString();

        }

    }
}
