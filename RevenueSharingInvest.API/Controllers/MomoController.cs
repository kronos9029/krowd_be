using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Business.Helpers;
using RevenueSharingInvest.Business.Models;
using RevenueSharingInvest.Business.Models.Constant;
using RevenueSharingInvest.Business.Services;
using RevenueSharingInvest.Business.Services.Extensions.Momo;
using RevenueSharingInvest.Business.Services.Impls;
using RevenueSharingInvest.Data.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;    

namespace RevenueSharingInvest.API.Controllers
{
    [ApiController]
    [Route("api/v1.0/momo")]
    [EnableCors]
    [AllowAnonymous]
    public class MomoController : ControllerBase
    {
        private readonly IAccountTransactionService _accountTransactionService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMomoService _momoService;
        private readonly IRoleService _roleService;
        private readonly IUserService _userService;
        private readonly MomoSettings _momoSettings;

        public MomoController(IOptions<MomoSettings> momoSettings, 
            IAccountTransactionService accountTransactionService, 
            IHttpContextAccessor httpContextAccessor, 
            IMomoService momoService,
            IRoleService roleService,
            IUserService userService)
        {
            _accountTransactionService = accountTransactionService;
            _httpContextAccessor = httpContextAccessor;
            _momoSettings = momoSettings.Value;
            _momoService = momoService;
            _roleService = roleService;
            _userService = userService;
        }
/*
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
                "&ipnUrl=" + notifyurl +
                "&orderId=" + orderid +
                "&orderInfo=" + orderInfo +
                "&partnerCode=" + partnerCode +
                "&redirectUrl=" + "https://www.krowd.vn/page-success" +
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
                { "redirectUrl", "https://www.krowd.vn/page-success" },
                { "ipnUrl", notifyurl },
                { "lang", "vi" },
                { "extraData", extraData },
                { "requestType", requestType },
                { "signature", signature }

            };

            string responseFromMomo = PaymentRequest.sendPaymentRequest(endpoint, message.ToString());

            JObject jmessage = JObject.Parse(responseFromMomo);

            return Ok(jmessage.GetValue("payUrl").ToString());

        }        */
        
        [HttpPost]
        [Route("request")]
        public async Task<IActionResult> RequestPaymentTest([FromForm]long amount)
        {
            MomoPaymentRequest request = new();
            request.amount = amount;
            ThisUserObj currentUser = await GetThisUserInfo(HttpContext);
            if (currentUser.email == null 
                || currentUser.email == "" 
                || currentUser.userId == null 
                || currentUser.userId == "")
            {
                throw new LoginException("You Have To Login First!!");
            }
            request.partnerClientId = currentUser.userId;
            request.email = currentUser.email;

            var result = _momoService.RequestPaymentWeb(request);
            return Ok(result);

        }        
        
/*        [HttpPost]
        [Route("request-mobile")]
        public async Task<IActionResult> RequestLinkAndPay(MomoPaymentRequest request)
        {
            ThisUserObj currentUser = await GetThisUserInfo(HttpContext);
            request.partnerClientId = currentUser.userId;
            request.email = currentUser.email;
            var result = _momoService.RequestLinkAndPayment(request);
            return Ok(result);

        }*/

        [HttpPost]
        [Route("response")]
        public async Task<IActionResult> ResponseFromMomo(MomoPaymentResult momoPaymentResult)
        {
            var result = await _accountTransactionService.CreateAccountTransaction(momoPaymentResult);
            return Ok(result);
        }

        [HttpPost]
        [Route("query")]
        public async Task<IActionResult> QueryTransactionStatus(QueryRequest request)
        {
            var result = await _momoService.QueryTransactionStatus(request);
            return Ok(result);
        }

        [HttpPost]
        [Route("confirm")]
        public async Task<IActionResult> ConfirmMomoTransaction(ConfirmRequest request)
        {
            var result = await _momoService.ConfirmMomoTransaction(request);
            return Ok(result);
        }

        private async Task<ThisUserObj> GetThisUserInfo(HttpContext? httpContext)
        {
            ThisUserObj currentUser = new();

            var checkUser = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.SerialNumber);
            if (checkUser == null)
            {
                currentUser.userId = "";
                currentUser.email = "";
                currentUser.investorId = "";
            }else
            {
                currentUser.userId = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.SerialNumber).Value;
                currentUser.email = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
                currentUser.investorId = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.GroupSid).Value;
            }

            List<RoleDTO> roleList = await _roleService.GetAllRoles();
            GetUserDTO? userDTO = await _userService.GetUserByEmail(currentUser.email);
            if (userDTO == null)
            {
                currentUser.roleId = "";
                currentUser.businessId = "";

            }
            else
            {
                currentUser.roleId = userDTO.role.id;
                if (userDTO.business != null)
                {
                    currentUser.businessId = userDTO.business.id;
                }
                else
                {
                    currentUser.businessId = "";
                }

            }
            foreach (RoleDTO role in roleList)
            {
                if (role.name.Equals(Enum.GetNames(typeof(RoleEnum)).ElementAt(0)))
                {
                    currentUser.adminRoleId = role.id;
                }
                if (role.name.Equals(Enum.GetNames(typeof(RoleEnum)).ElementAt(3)))
                {
                    currentUser.investorRoleId = role.id;
                }
                if (role.name.Equals(Enum.GetNames(typeof(RoleEnum)).ElementAt(1)))
                {
                    currentUser.businessManagerRoleId = role.id;
                }
                if (role.name.Equals(Enum.GetNames(typeof(RoleEnum)).ElementAt(2)))
                {
                    currentUser.projectManagerRoleId = role.id;
                }
            }

            return currentUser;

        }

    }
}
