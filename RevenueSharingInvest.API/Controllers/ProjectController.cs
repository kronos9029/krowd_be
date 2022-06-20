﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RevenueSharingInvest.Business.Models.Constant;
using RevenueSharingInvest.Business.Services;
using RevenueSharingInvest.Data.Models.DTOs;
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
    [Route("api/v1.0/projects")]
    [EnableCors]
        [Authorize]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IAuthenticateService _authenticateService;
        public ProjectController(IProjectService projectService, IHttpContextAccessor httpContextAccessor, IAuthenticateService authenticateService)
        {
            _projectService = projectService;
            this.httpContextAccessor = httpContextAccessor;
            _authenticateService = authenticateService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] ProjectDTO projectDTO)
        {
            string userId = httpContextAccessor.HttpContext.User.Claims.First(c => c.Type == ClaimTypes.SerialNumber).Value;

            if(await _authenticateService.CheckRoleForUser(userId, RoleEnum.BUSINESS_MANAGER.ToString()))
            {
                var result = await _projectService.CreateProject(projectDTO);
                return Ok(result);
            }
            return StatusCode((int)HttpStatusCode.Forbidden, "You Don't Have Permission To Do This");
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProjects(int pageIndex, int pageSize)
        {
            var result = new List<ProjectDTO>();
            result = await _projectService.GetAllProjects(pageIndex, pageSize);
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetProjectById(Guid id)
        {
            ProjectDTO dto = new();
            dto = await _projectService.GetProjectById(id);
            return Ok(dto);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateProject([FromBody] ProjectDTO projectDTO, Guid id)
        {
            string userId = httpContextAccessor.HttpContext.User.Claims.First(c => c.Type == ClaimTypes.SerialNumber).Value;

            if (await _authenticateService.CheckRoleForUser(userId, RoleEnum.BUSINESS_MANAGER.ToString()))
            {
                var result = await _projectService.UpdateProject(projectDTO, id);
                return Ok(result);
            }
            return StatusCode((int)HttpStatusCode.Forbidden, "You Don't Have Permission To Do This");
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteProject(Guid id)
        {
            string userId = httpContextAccessor.HttpContext.User.Claims.First(c => c.Type == ClaimTypes.SerialNumber).Value;

            if (await _authenticateService.CheckRoleForUser(userId, RoleEnum.ADMIN.ToString()))
            {
                var result = await _projectService.DeleteProjectById(id);
                return Ok(result);
            }
            return StatusCode((int)HttpStatusCode.Forbidden, "You Don't Have Permission To Do This");

        }

        [HttpDelete]
        public async Task<IActionResult> ClearAllProjectData()
        {
            string userId = httpContextAccessor.HttpContext.User.Claims.First(c => c.Type == ClaimTypes.SerialNumber).Value;

            if (await _authenticateService.CheckRoleForUser(userId, RoleEnum.ADMIN.ToString()))
            {
                var result = await _projectService.ClearAllProjectData();
                return Ok(result);
            }
            return StatusCode((int)HttpStatusCode.Forbidden, "You Don't Have Permission To Do This");

        }
    }
}
