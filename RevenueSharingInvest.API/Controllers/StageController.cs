using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RevenueSharingInvest.API.Extensions;
using RevenueSharingInvest.Business.Models.Constant;
using RevenueSharingInvest.Business.Services;
using RevenueSharingInvest.Data.Models.DTOs;
using RevenueSharingInvest.Data.Models.DTOs.CommonDTOs;
using RevenueSharingInvest.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RevenueSharingInvest.API.Controllers
{
    [ApiController]
    [Route("api/v1.0/stages")]
    [EnableCors]
    public class StageController : ControllerBase
    {
        private readonly IStageService _stageService;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IHttpContextAccessor httpContextAccessor;
        public StageController(IStageService stageService, IUserService userService, IRoleService roleService, IHttpContextAccessor httpContextAccessor)
        {
            _stageService = stageService;
            _userService = userService;
            _roleService = roleService;
            this.httpContextAccessor = httpContextAccessor;
        }

        //GET BY PROJECT ID
        [HttpGet]
        [Route("project/{project_id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllStagesByProjectId(Guid project_id, int pageIndex, int pageSize)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);

            //ALL ROLES
            if (currentUser.roleId.Equals(currentUser.adminRoleId)
                || currentUser.roleId.Equals(currentUser.businessManagerRoleId)
                || currentUser.roleId.Equals(currentUser.projectManagerRoleId)
                || currentUser.roleId.Equals(currentUser.investorRoleId)
                || currentUser.roleId.Equals(""))
            {
                var result = new AllStageDTO();
                result = await _stageService.GetAllStagesByProjectId(project_id, pageIndex, pageSize, currentUser);
                return Ok(result);
            }

            return StatusCode((int)HttpStatusCode.Forbidden, "You Don't Have Permission Perform This Action!!");
        }

        //GET CHART
        [HttpGet]
        [Route("chart/{project_id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetStageChartByProjectId(Guid project_id)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);

            //ALL ROLES
            if (currentUser.roleId.Equals(currentUser.adminRoleId)
                || currentUser.roleId.Equals(currentUser.businessManagerRoleId)
                || currentUser.roleId.Equals(currentUser.projectManagerRoleId)
                || currentUser.roleId.Equals(currentUser.investorRoleId)
                || currentUser.roleId.Equals(""))
            {
                var result = new List<StageChartDTO>();
                result = await _stageService.GetStageChartByProjectId(project_id, currentUser);
                return Ok(result);
            }

            return StatusCode((int)HttpStatusCode.Forbidden, "You Don't Have Permission Perform This Action!!");
        }

        //GET BY ID
        [HttpGet]
        [Route("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetStageById(Guid id)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);

            //ALL ROLES
            if (currentUser.roleId.Equals(currentUser.adminRoleId)
                || currentUser.roleId.Equals(currentUser.businessManagerRoleId)
                || currentUser.roleId.Equals(currentUser.projectManagerRoleId)
                || currentUser.roleId.Equals(currentUser.investorRoleId)
                || currentUser.roleId.Equals(""))
            {
                var result = new GetStageDTO();
                result = await _stageService.GetStageById(id, currentUser);
                return Ok(result);
            }

            return StatusCode((int)HttpStatusCode.Forbidden, "You Don't Have Permission Perform This Action!!");
        }

        //UPDATE
        [HttpPut]
        [Route("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateStage([FromBody] UpdateStageDTO stageDTO, Guid id)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);

            //PROJECT_MANAGER
            if (currentUser.roleId.Equals(currentUser.projectManagerRoleId))
            {
                var result = await _stageService.UpdateStage(stageDTO, id, currentUser);
                return Ok(result);
            }

            return StatusCode((int)HttpStatusCode.Forbidden, "Only user with role PROJECT_MANAGER can perform this action!!!");
        }
    }
}
