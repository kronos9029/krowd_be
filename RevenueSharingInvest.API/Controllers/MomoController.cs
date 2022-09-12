using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using RevenueSharingInvest.Business.Helpers;
using RevenueSharingInvest.Business.Models;
using RevenueSharingInvest.Business.Services.Extensions.Momo;
using System;
using System.Threading.Tasks;

namespace RevenueSharingInvest.API.Controllers
{
    [ApiController]
    [Route("api/v1.0/momo")]
    [EnableCors]
    [AllowAnonymous]
    public class MomoController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly MomoSettings _momoSettings;

        public MomoController(IOptions<MomoSettings> momoSettings, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _momoSettings = momoSettings.Value;
        }

        [HttpPost]
        [Route("request")]
        public async Task<IActionResult> RequestPayment(MomoPaymentRequest request)
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
                "&ipnUrl=" + "https://docs.google.com/spreadsheets/d/1X3omZg__GAkcntodcC783eiFcBMWEibJXOaJn36rAIU/edit#gid=0" +
                "&orderId=" + orderid +
                "&orderInfo=" + orderInfo +
                "&partnerCode=" + partnerCode +
                "&redirectUrl=" + "https://docs.google.com/spreadsheets/d/1X3omZg__GAkcntodcC783eiFcBMWEibJXOaJn36rAIU/edit#gid=0" +
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
                { "redirectUrl", "https://docs.google.com/spreadsheets/d/1X3omZg__GAkcntodcC783eiFcBMWEibJXOaJn36rAIU/edit#gid=0" },
                { "ipnUrl", "https://docs.google.com/spreadsheets/d/1X3omZg__GAkcntodcC783eiFcBMWEibJXOaJn36rAIU/edit#gid=0" },
                { "lang", "en" },
                { "extraData", extraData },
                { "requestType", requestType },
                { "signature", signature }

            };

            string responseFromMomo = PaymentRequest.sendPaymentRequest(endpoint, message.ToString());

            JObject jmessage = JObject.Parse(responseFromMomo);

            return Ok(jmessage.GetValue("payUrl").ToString());

        }

        [HttpPost]
        [Route("confirm")]
        public async Task<IActionResult> ConfirmPaymentClient(MomoPaymentRequest result)
        {
            

            return Ok(result);
        }

    }
}
