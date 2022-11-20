using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RevenueSharingInvest.API.Extensions;
using RevenueSharingInvest.Business.Services;
using RevenueSharingInvest.Business.Services.Extensions.Firebase;
using RevenueSharingInvest.Business.Services.Impls;
using System;
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
        private readonly IFileUploadService _fileUploadService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IProjectService _projectService;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;

        public AuthenticateController(IAuthenticateService authenticateService, 
            IFileUploadService fileUploadService, 
            IHttpContextAccessor httpContextAccessor, 
            IProjectService projectService,
            IRoleService roleService,
            IUserService userService)
        {
            _authenticateService = authenticateService;
            _fileUploadService = fileUploadService;
            _httpContextAccessor = httpContextAccessor;
            _projectService = projectService;
            _roleService = roleService;
            _userService = userService;
        }

        [HttpPost]
        [Route("investor")]
        public async Task<IActionResult> GetTokenInvestor([FromQuery] string token, string deviceToken)
        {
            var result = await _authenticateService.GetTokenInvestor(token, deviceToken);
            return Ok(result);
        }

        [HttpPost]
        [Route("business")]
        public async Task<IActionResult> GetTokenBusiness([FromQuery] string token)
        {
            var result = await _authenticateService.GetTokenWebBusiness(token);
            return Ok(result);
        }


        //[ServiceFilter(typeof(ClientIpCheckActionFilter))]
        [HttpPost]
        [Route("admin")]
        public async Task<IActionResult> GetTokenAdmin([FromQuery] string token)
        {
            var result = await _authenticateService.GetTokenAdmin(token);
            return Ok(result);
        }

        [HttpPost]
        [Route("integrate-info")]
        [Authorize]
        public async Task<IActionResult> GetIntegrateInfo([FromQuery] string projectId)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);
            var result = await _userService.GetIntegrateInfoByEmailAndProjectId(currentUser.email, projectId);
            return Ok(result);
        }
        
        [HttpPost]
        [Route("logout")]
        [Authorize]
        public async Task<IActionResult> Logout([FromQuery] string deviceToken)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);
            await _authenticateService.Logout(currentUser.userId, deviceToken);
            return Ok(0);
        }
/*
        [HttpPost]
        [Route("test")]
        public async Task<IActionResult> UploadFile()
        {
            TimeZoneInfo timeZoneInfo;
            DateTime dateTime;
            //Set the time zone information to US Mountain Standard Time 
            timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
            //Get date and time in US Mountain Standard Time 
            dateTime = TimeZoneInfo.ConvertTime(DateTime.Now, timeZoneInfo);
            //Print out the date and time
            string timeString = dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
            DateTime now = DateTime.Parse(timeString);
            return Ok(now);
        }*/


    }
}
