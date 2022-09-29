using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RevenueSharingInvest.Business.Services;
using RevenueSharingInvest.Business.Services.Extensions.Firebase;
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

        public AuthenticateController(IAuthenticateService authenticateService, IFileUploadService fileUploadService, IHttpContextAccessor httpContextAccessor)
        {
            _authenticateService = authenticateService;
            _fileUploadService = fileUploadService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        [Route("investor")]
        public async Task<IActionResult> GetTokenInvestor([FromQuery] string token)
        {
            var result = await _authenticateService.GetTokenInvestor(token);
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
        }


    }
}
