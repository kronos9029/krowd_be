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
    [Route("api/v1.0/User")]
    [EnableCors]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        public UserController(IUserService userService, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("authenticate-mobile")]
        public async Task<IActionResult> AuthenticateMobile([FromQuery]string token)
        {
            var result = await _userService.GetTokenInvestor(token);
            return Ok(result);
        }
        
        [HttpPost]
        [AllowAnonymous]
        [Route("authenticate-web")]
        public async Task<IActionResult> AuthenticateWeb([FromQuery]string token)
        {
            var result = await _userService.GetTokenWebBusiness(token);
            return Ok(result);
        }

    }
}
