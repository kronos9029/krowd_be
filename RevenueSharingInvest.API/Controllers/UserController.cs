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

<<<<<<< Updated upstream
        //[HttpPost]
        //[AllowAnonymous]
        //[Route("authenticate-user")]
        //public async Task<IActionResult> AuthenticateUser([FromQuery]string token)
        //{
        //    var result = await _userService.AuthenticateUser(token);
        //    return Ok(result);
        //}
=======
        [HttpPost]
        [AllowAnonymous]
        [Route("authenticate-user")]
        public async Task<IActionResult> AuthenticateUser([FromQuery]string token)
        {
            var result = await _userService.GetTokenMobileInvestor(token);
            return Ok(result);
        }
>>>>>>> Stashed changes
    }
}
