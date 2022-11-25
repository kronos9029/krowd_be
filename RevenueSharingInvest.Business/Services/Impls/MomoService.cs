using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RevenueSharingInvest.Business.Helpers;
using RevenueSharingInvest.Business.Models;
using RevenueSharingInvest.Business.Services.Extensions;
using RevenueSharingInvest.Data.Helpers.Logger;
using RevenueSharingInvest.Business.Services.Extensions.Momo;
using RevenueSharingInvest.Data.Repositories.IRepos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using static Google.Apis.Requests.BatchRequest;
using RevenueSharingInvest.Business.Models.Constant;
using RevenueSharingInvest.Business.Exceptions;
using AutoMapper.Execution;

namespace RevenueSharingInvest.Business.Services.Impls
{
    public class MomoService : IMomoService
    {
        private readonly MomoSettings _momoSettings;
        public MomoService(IOptions<MomoSettings> momoSettings)
        {
            _momoSettings = momoSettings.Value;
        }

        public async Task<MomoPaymentResponse> RequestPaymentWeb(MomoPaymentRequest request)
        {
            try
            {

                if (request.amount > 50000000)
                    throw new AmountExcessException("Maximum amount is 50000000 VND");
                //request params need to request to MoMo system
                string partnerCode = _momoSettings.PartnerCode;
                string apiEndpoint = _momoSettings.ApiEndpoint;
                string accessKey = _momoSettings.AccessKey;
                string serectkey = _momoSettings.SecretKey;
                string orderInfo = "";
                
                string notifyurl = _momoSettings.NotifyUrl; //lưu ý: notifyurl không được sử dụng localhost, có thể sử dụng ngrok để public localhost trong quá trình test

                long amount = request.amount;
                string orderid = Guid.NewGuid().ToString(); //mã đơn hàng
                string requestId = Guid.NewGuid().ToString();
                string extraData = "";
                string requestType = "captureWallet";
                dynamic userInfoJson = new JObject();
                userInfoJson.Add("name", "");
                userInfoJson.Add("phoneNumber", "");
                userInfoJson.Add("email", request.email);

                string returnUrl = "";
                if (request.role.Equals(RoleEnum.INVESTOR.ToString()))
                {
                    orderInfo = "Nạp tiền vào ví đầu tư chung.";
                    returnUrl = _momoSettings.InvestorReturnUrl;
                }
                                    
                if (request.role.Equals(RoleEnum.PROJECT_MANAGER.ToString()))
                {
                    orderInfo = "Nạp tiền vào ví thanh toán chung.";
                    returnUrl = _momoSettings.ProjectManagerReturnUrl;
                }
                    

                //Before sign HMAC SHA256 signature
                string rawHash = "accessKey=" + accessKey +
                    "&amount=" + amount +
                    "&extraData=" + extraData +
                    "&ipnUrl=" + notifyurl +
                    "&orderId=" + orderid +
                    "&orderInfo=" + orderInfo +
                    "&partnerClientId=" + request.partnerClientId +
                    "&partnerCode=" + partnerCode +
                    "&redirectUrl=" + returnUrl +
                    "&requestId=" + requestId +
                    "&requestType=" + requestType
                    ;

                MomoSecurity crypto = new MomoSecurity();
                //sign signature SHA256
                string signature = crypto.signSHA256(rawHash, serectkey);


                //build body json request
                JObject message = new()
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
                    { "partnerClientId", request.partnerClientId },
                    { "userInfo", userInfoJson },
                    { "signature", signature }
                };

                string responseFromMomo = PaymentRequest.sendPaymentRequest(apiEndpoint, message.ToString());

                JObject jmessage = JObject.Parse(responseFromMomo);

                MomoPaymentResponse response = jmessage.ToObject<MomoPaymentResponse>();

                //JsonSerializer serializer = new JsonSerializer();
                //MomoPaymentResponse response = (MomoPaymentResponse)serializer.Deserialize(new JTokenReader(jmessage), typeof(MomoPaymentResponse));

                return response;
            } catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }


        }

        public async Task<MomoPaymentResponse> RequestLinkAndPayment(MomoPaymentRequest request)
        {
            try
            {
                //request params need to request to MoMo system
                string partnerCode = _momoSettings.PartnerCode;
                string accessKey = _momoSettings.AccessKey;
                string apiEndpoint = _momoSettings.ApiEndpoint;
                string serectkey = _momoSettings.SecretKey;
                string requestId = Guid.NewGuid().ToString();
                long amount = request.amount;
                string orderid = Guid.NewGuid().ToString(); //mã đơn hàng
                string notifyurl = _momoSettings.NotifyUrl; //lưu ý: notifyurl không được sử dụng localhost, có thể sử dụng ngrok để public localhost trong quá trình test
                string extraData = "";
                string requestType = "linkWallet";
                string orderInfo = "test Link and Pay";

                string returnUrl = "";
                if (request.role.Equals(RoleEnum.INVESTOR.ToString()))
                    returnUrl = _momoSettings.InvestorReturnUrl;
                if (request.role.Equals(RoleEnum.PROJECT_MANAGER.ToString()))
                    returnUrl = _momoSettings.ProjectManagerReturnUrl;

                string rawHash = "accessKey=" + accessKey +
                                "&amount=" + amount +
                                "&extraData=" + extraData +
                                "&ipnUrl=" + notifyurl +
                                "&orderId=" + orderid +
                                "&orderInfo=" + orderInfo +
                                "&partnerClientId=" + request.email +
                                "&partnerCode=" + partnerCode +
                                "&redirectUrl=" + returnUrl +
                                "&requestId=" + requestId +
                                "&requestType=" + requestType
                                ;

                MomoSecurity crypto = new MomoSecurity();
                //sign signature SHA256
                string signature = crypto.signSHA256(rawHash, serectkey);


                JObject message = new JObject
            {
                { "partnerCode", partnerCode },
                { "partnerName", "Revenue Sharing Invest" },
                { "storeId", partnerCode },
                { "requestType", requestType },
                { "ipnUrl", notifyurl },
                { "redirectUrl", returnUrl },
                { "orderId", orderid },
                { "amount", amount },
                { "partnerClientId", request.email },
                { "lang", "vi" },
                { "orderInfo", orderInfo},
                { "requestId", requestId },
                { "extraData", "" },
                { "signature", signature }

            };

                string responseFromMomo = PaymentRequest.sendPaymentRequest(apiEndpoint, message.ToString());

                JObject jmessage = JObject.Parse(responseFromMomo);

                MomoPaymentResponse response = jmessage.ToObject<MomoPaymentResponse>();

                return response;
            }
            catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }


        }

        public async Task<QueryResponse> QueryTransactionStatus(QueryRequest request)
        {
            try
            {
                string partnerCode = _momoSettings.PartnerCode;
                string accessKey = _momoSettings.AccessKey;
                string requestId = request.requestId;
                string orderId = request.orderId;
                string secrectKey = _momoSettings.SecretKey;
                string apiEndpoint = _momoSettings.ApiQueryStatus;
                string lang = "vi";

                string rawHash = "accessKey=" + accessKey +
                    "&orderId=" + orderId +
                    "&partnerCode=" + partnerCode +
                    "&requestId=" + requestId
                    ;

                MomoSecurity crypto = new MomoSecurity();
                //sign signature SHA256
                string signature = crypto.signSHA256(rawHash, secrectKey);

                JObject message = new JObject
            {
                { "partnerCode", partnerCode },
                { "requestId", requestId },
                { "orderId", orderId },
                { "lang", "vi" },
                { "signature", signature }
            };

                string responseFromMomo = PaymentRequest.sendPaymentRequest(apiEndpoint, message.ToString());

                JObject jmessage = JObject.Parse(responseFromMomo);

                QueryResponse response = jmessage.ToObject<QueryResponse>();

                return response;
            }
            catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }


        }

        public async Task<ConfirmResponse> ConfirmMomoTransaction(ConfirmRequest request)
        {
            try
            {
                string partnerCode = _momoSettings.PartnerCode;
                string accessKey = _momoSettings.AccessKey;
                string requestId = request.requestId;
                string orderId = request.orderId;
                string secrectKey = _momoSettings.SecretKey;
                string apiEndpoint = _momoSettings.ApiQueryStatus;
                string lang = "vi";

                string rawHash = "accessKey=" + accessKey +
                    "&amount=" + request.amount +
                    "&amount=" + request.description +
                    "&orderId=" + orderId +
                    "&partnerCode=" + partnerCode +
                    "&requestId=" + requestId +
                    "&requestType=" + request.requestType
                    ;

                MomoSecurity crypto = new MomoSecurity();
                //sign signature SHA256
                string signature = crypto.signSHA256(rawHash, secrectKey);

                JObject message = new JObject
            {
                { "partnerCode", partnerCode },
                { "requestId", requestId },
                { "orderId", orderId },
                { "requestType", request.requestType },
                { "amount", request.amount },
                { "lang", "vi" },
                { "description", request.description },
                { "signature", signature }
            };

                string responseFromMomo = PaymentRequest.sendPaymentRequest(apiEndpoint, message.ToString());

                JObject jmessage = JObject.Parse(responseFromMomo);

                ConfirmResponse response = jmessage.ToObject<ConfirmResponse>();

                return response;
            }
            catch(Exception e)
            {
                LoggerService.Logger(e.ToString());
                throw new Exception(e.Message);
            }

        }

    }
}
