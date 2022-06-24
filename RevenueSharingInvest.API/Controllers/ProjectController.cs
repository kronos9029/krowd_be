using Microsoft.AspNetCore.Authorization;
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
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projectService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthenticateService _authenticateService;
        public ProjectController(IProjectService projectService, IHttpContextAccessor httpContextAccessor, IAuthenticateService authenticateService)
        {
            _projectService = projectService;
            _httpContextAccessor = httpContextAccessor;
            _authenticateService = authenticateService;
        }

        [HttpPost]
        //[Authorize]
        public async Task<IActionResult> CreateProject([FromBody] ProjectDTO projectDTO)
        {
            //string userId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.SerialNumber).Value;
            //if (await _authenticateService.CheckRoleForAction(userId, RoleEnum.BUSINESS_MANAGER.ToString()))
            //{
                var result = await _projectService.CreateProject(projectDTO);
                return Ok(result);
            //}
            //return StatusCode((int)HttpStatusCode.Forbidden, "You Don't Have Permission Perform This Action!!");
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProjects(int pageIndex, int pageSize, string businessId, string temp_field_role)
        {
            var result = new AllProjectDTO();
            result = await _projectService.GetAllProjects(pageIndex, pageSize, businessId, temp_field_role);
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
        //[Authorize]
        public async Task<IActionResult> UpdateProject([FromBody] ProjectDTO projectDTO, Guid id)
        {
            //string userId = _httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.SerialNumber).Value;

            //if (await _authenticateService.CheckRoleForAction(userId, RoleEnum.BUSINESS_MANAGER.ToString()))
            //{
                var result = await _projectService.UpdateProject(projectDTO, id);
                return Ok(result);
            //}
            //return StatusCode((int)HttpStatusCode.Forbidden, "You Don't Have Permission Perform This Action!!");
        }

        [HttpDelete]
        [Route("{id}")]
        //[Authorize]
        public async Task<IActionResult> DeleteProject(Guid id)
        {
            //string userId = _httpContextAccessor.HttpContext.User.Claims.First(c => c.Type == ClaimTypes.SerialNumber).Value;

            //if (await _authenticateService.CheckRoleForAction(userId, RoleEnum.ADMIN.ToString()) && await _authenticateService.CheckIdForAction(userId, id))
            //{
                var result = await _projectService.DeleteProjectById(id);
                return Ok(result);
            //}
            //return StatusCode((int)HttpStatusCode.Forbidden, "You Don't Have Permission Perform This Action!!");

        }

        [HttpDelete]
        //[Authorize]
        public async Task<IActionResult> ClearAllProjectData()
        {
            //string userId = _httpContextAccessor.HttpContext.User.Claims.First(c => c.Type == ClaimTypes.SerialNumber).Value;

            //if (await _authenticateService.CheckRoleForAction(userId, RoleEnum.ADMIN.ToString()))
            //{
                var result = await _projectService.ClearAllProjectData();
                return Ok(result);
            //}
            //return StatusCode((int)HttpStatusCode.Forbidden, "You Don't Have Permission Perform This Action!!");

        }
    }
}
