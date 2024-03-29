﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RevenueSharingInvest.API.Extensions;
using RevenueSharingInvest.Business.Services;
using RevenueSharingInvest.Data.Models.DTOs;
using RevenueSharingInvest.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace RevenueSharingInvest.API.Controllers
{
    [ApiController]
    [Route("api/v1.0/period_revenue_histories")]
    [EnableCors]
    //[Authorize]
    public class PeriodRevenueHistoryController : ControllerBase
    {
        private readonly IPeriodRevenueHistoryService _periodRevenueHistoryService;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public PeriodRevenueHistoryController(IPeriodRevenueHistoryService periodRevenueHistoryService, IUserService userService, IRoleService roleService, IHttpContextAccessor httpContextAccessor)
        {
            _periodRevenueHistoryService = periodRevenueHistoryService;
            _userService = userService;
            _roleService = roleService;

            _httpContextAccessor = httpContextAccessor;
        }

        //CREATE
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreatePeriodRevenueHistory([FromBody] CreatePeriodRevenueHistoryDTO createPeriodRevenueHistoryDTO)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);

            //PROJECT_MANAGER
            if (currentUser.roleId.Equals(currentUser.projectManagerRoleId))
            {
                var result = await _periodRevenueHistoryService.CreatePeriodRevenueHistory(createPeriodRevenueHistoryDTO, currentUser);
                return Ok(result);
            }

            return StatusCode((int)HttpStatusCode.Forbidden, "Only user with role PROJECT_MANAGER can perform this action!!!");
        }

        //GET ALL
        [HttpGet]
        [Route("project/{project_id}")]
        public async Task<IActionResult> GetAllPeriodRevenueHistorys(int pageIndex, int pageSize, Guid project_id)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);

            //ADMIN, BUSINESS_MANAGER, PROJECT_MANAGER
            if (currentUser.roleId.Equals(currentUser.adminRoleId)
                || currentUser.roleId.Equals(currentUser.businessManagerRoleId)
                || currentUser.roleId.Equals(currentUser.projectManagerRoleId))
            {
                var result = await _periodRevenueHistoryService.GetAllPeriodRevenueHistories(pageIndex, pageSize, project_id, currentUser);
                return Ok(result);
            }
            return StatusCode((int)HttpStatusCode.Forbidden, "Only user with role ADMIN or BUSINESS_MANAGER or PROJECT_MANAGER can perform this action!!!");
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetPeriodRevenueHistoryById(Guid id)
        {
            PeriodRevenueHistoryDTO dto = new PeriodRevenueHistoryDTO();
            dto = await _periodRevenueHistoryService.GetPeriodRevenueHistoryById(id);
            return Ok(dto);
        }
    }
}
