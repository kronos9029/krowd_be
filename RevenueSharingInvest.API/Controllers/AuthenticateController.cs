using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using RevenueSharingInvest.Business.Helpers;
using RevenueSharingInvest.Business.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RevenueSharingInvest.API.Controllers
{
    [ApiController]
    [Route("api/v1.0/authenticate")]
    [EnableCors]
    [AllowAnonymous]
    public class AuthenticateController : ControllerBase
    {
        private readonly IAuthenticateService _authenticateService;

        public AuthenticateController(IAuthenticateService authenticateService)
        {
            _authenticateService = authenticateService;
        }

        [HttpPost]
        [Route("investor")]
        public async Task<IActionResult> GetTokenInvestor([FromQuery] string token)
        {
/*            var result = await _authenticateService.GetTokenInvestor(token);
            return Ok(result);*/
            var remoteIpAddress = HttpContext.Connection.LocalIpAddress;
            //var result = await _authenticateService.GetTokenWebBusiness(token);
            string ip = GetClientIp(HttpContext);
            return Ok(ip);
        }
        private String GetClientIp(HttpContext context)
        {
            string ip = string.Empty;
            if (!string.IsNullOrEmpty(context.Request.Headers["X-Forwarded-For"]))
            {
                ip = context.Request.Headers["X-Forwarded-For"];
            }
            else
            {
                ip = context.Request.HttpContext.Features.Get<IHttpConnectionFeature>().RemoteIpAddress.ToString();
            }
            return ip;
        }

        [ServiceFilter(typeof(ClientIpCheckActionFilter))]
        [HttpPost]
        [Route("business")]
        public async Task<IActionResult> GetTokenWebBusiness([FromQuery] string token)
        {
            var remoteIpAddress = HttpContext.Connection.RemoteIpAddress;
            //var result = await _authenticateService.GetTokenWebBusiness(token);
            return Ok(remoteIpAddress.ToString());
        }
    }
}
