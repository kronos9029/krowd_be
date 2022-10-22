using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RevenueSharingInvest.API.Extensions;
using RevenueSharingInvest.Business.Services;
using RevenueSharingInvest.Data.Models.DTOs;
using RevenueSharingInvest.Data.Models.DTOs.CommonDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace RevenueSharingInvest.API.Controllers
{
    [ApiController]
    [Route("api/v1.0/daily_reports")]
    [EnableCors]
    public class DailyReportController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthenticateService _authenticateService;
        private readonly IDailyReportService _dailyReportService;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;

        public DailyReportController(IDailyReportService dailyReportService, IUserService userService, IRoleService roleService, IHttpContextAccessor httpContextAccessor, IAuthenticateService authenticateService)
        {
            _dailyReportService = dailyReportService;
            _userService = userService;
            _roleService = roleService;
            _httpContextAccessor = httpContextAccessor;
            _authenticateService = authenticateService;
        }

        //GET ALL
        [HttpGet]
        [Route("project/{projectId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllDailyReports(int pageIndex, int pageSize, Guid projectId, Guid? stageId)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);

            //ALL ROLES
            if (currentUser.roleId.Equals(currentUser.adminRoleId)
                || currentUser.roleId.Equals(currentUser.businessManagerRoleId)
                || currentUser.roleId.Equals(currentUser.projectManagerRoleId)
                || currentUser.roleId.Equals(currentUser.investorRoleId)
                || currentUser.roleId.Equals(""))
            {
                var result = new AllDailyReportDTO();
                result = await _dailyReportService.GetAllDailyReports(pageIndex, pageSize, projectId, stageId, currentUser);
                return Ok(result);
            }

            return StatusCode((int)HttpStatusCode.Forbidden, "You Don't Have Permission Perform This Action!!");
        }

        //GET BY ID
        [HttpGet]
        [Route("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDailyReportById(Guid id)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);

            //ALL ROLES
            if (currentUser.roleId.Equals(currentUser.adminRoleId)
                || currentUser.roleId.Equals(currentUser.businessManagerRoleId)
                || currentUser.roleId.Equals(currentUser.projectManagerRoleId)
                || currentUser.roleId.Equals(currentUser.investorRoleId)
                || currentUser.roleId.Equals(""))
            {
                var result = new DailyReportDTO();
                result = await _dailyReportService.GetDailyReportById(id, currentUser);
                return Ok(result);
            }

            return StatusCode((int)HttpStatusCode.Forbidden, "You Don't Have Permission Perform This Action!!");
        }
    }
}
