using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RevenueSharingInvest.API.Extensions;
using RevenueSharingInvest.Business.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace RevenueSharingInvest.API.Controllers
{
    [ApiController]
    [Route("api/v1.0/money_overview")]
    [EnableCors]
    public class MoneyOverviewController : ControllerBase
    {
        private readonly IMoneyOverviewService _moneyOverviewService;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthenticateService _authenticateService;
        public MoneyOverviewController(IMoneyOverviewService moneyOverviewService, IUserService userService, IRoleService roleService, IHttpContextAccessor httpContextAccessor, IAuthenticateService authenticateService)
        {
            _moneyOverviewService = moneyOverviewService;
            _userService = userService;
            _roleService = roleService;
            _httpContextAccessor = httpContextAccessor;
            _authenticateService = authenticateService;
        }

        [HttpGet]
        [Route("investor")]
        [Authorize]
        public async Task<IActionResult> GetMoneyOverviewForInvestor()
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);

            if (currentUser.roleId.Equals(currentUser.investorRoleId))
            {
                var result = await _moneyOverviewService.GetMoneyOverviewForInvestor(currentUser);
                return Ok(result);
            }

            return StatusCode((int)HttpStatusCode.Forbidden, "Only user with role INVESTOR can perform this action!!!");
        }
    }
}
