using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public AuthenticateController()
        {
        }

        [HttpPost]
        [Route("business")]
        public async Task<IActionResult> GetTokenWebBusiness([FromQuery] string token)
        {
            var remoteIpAddress = HttpContext.Connection.RemoteIpAddress;
            return Ok(remoteIpAddress.ToString());
        }
    }
}
