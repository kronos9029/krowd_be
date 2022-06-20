﻿using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
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
    [Route("api/v1.0/authenticates")]
    [EnableCors]
    //[Authorize]
    public class AuthenticateController : ControllerBase
    {
        private readonly IAuthenticateService _authenticateService;

        public AuthenticateController(IAuthenticateService authenticateService)
        {
            _authenticateService = authenticateService;
        }

        [HttpPost]
        public async Task<IActionResult> GetTokenInvestor([FromQuery] string token)
        {
            var result = await _authenticateService.GetTokenInvestor(token);
            return Ok(result);
        }

        [HttpPost]
        [ServiceFilter(typeof(ClientIpCheckActionFilter))]
        public async Task<IActionResult> GetTokenWebBusiness([FromQuery] string token)
        {
            var result = await _authenticateService.GetTokenWebBusiness(token);
            return Ok(result);
        }
    }
}
