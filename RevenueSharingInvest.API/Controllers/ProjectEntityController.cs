using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RevenueSharingInvest.API.Extensions;
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
    [Route("api/v1.0/project_entities")]
    [EnableCors]    
    public class ProjectEntityController : ControllerBase
    {
        private readonly IProjectEntityService _projectEntityService;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IHttpContextAccessor httpContextAccessor;
        public ProjectEntityController(IProjectEntityService projectEntityService, IUserService userService, IRoleService roleService, IHttpContextAccessor httpContextAccessor)
        {
            _projectEntityService = projectEntityService;
            _userService = userService;
            _roleService = roleService;
            this.httpContextAccessor = httpContextAccessor;
        }

        //CREATE
        [HttpPost]
        [Authorize(Roles = "PROJECT_MANAGER")]
        public async Task<IActionResult> CreateProjectEntity([FromBody] CreateProjectEntityDTO projectEntityDTO)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);

            //PROJECT_MANAGER
            if (currentUser.roleId.Equals(currentUser.projectManagerRoleId))
            {
                var result = await _projectEntityService.CreateProjectEntity(projectEntityDTO, currentUser);
                return Ok(result);
            }
            return StatusCode((int) HttpStatusCode.Forbidden, "Only user with role PROJECT_MANAGER can perform this action!!!");
    }

        //GET ALL
        [HttpGet]
        [Route("project/{project_id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProjectEntityByProjectIdAndType(Guid project_id, string type)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);

            //ALL ROLES
            if (currentUser.roleId.Equals(currentUser.adminRoleId)
                || currentUser.roleId.Equals(currentUser.businessManagerRoleId)
                || currentUser.roleId.Equals(currentUser.projectManagerRoleId)
                || currentUser.roleId.Equals(currentUser.investorRoleId)
                || currentUser.roleId.Equals(""))
            {
                var result = new List<GetProjectEntityDTO>();
                result = await _projectEntityService.GetProjectEntityByProjectIdAndType(project_id, type, currentUser);
                return Ok(result);
            }

            return StatusCode((int)HttpStatusCode.Forbidden, "You Don't Have Permission Perform This Action!!");
        }

        //GET BY ID
        [HttpGet]
        [Route("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProjectEntityById(Guid id)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);

            //ALL ROLES
            if (currentUser.roleId.Equals(currentUser.adminRoleId)
                || currentUser.roleId.Equals(currentUser.businessManagerRoleId)
                || currentUser.roleId.Equals(currentUser.projectManagerRoleId)
                || currentUser.roleId.Equals(currentUser.investorRoleId)
                || currentUser.roleId.Equals(""))
            {
                GetProjectEntityDTO dto = new GetProjectEntityDTO();
                dto = await _projectEntityService.GetProjectEntityById(id, currentUser);
                return Ok(dto);
            }

            return StatusCode((int)HttpStatusCode.Forbidden, "You Don't Have Permission Perform This Action!!");
        }


        //UPDATE
        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = "PROJECT_MANAGER")]
        public async Task<IActionResult> UpdateProjectEntity([FromBody] UpdateProjectEntityDTO projectEntityDTO, Guid id)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);

            //PROJECT_MANAGER
            if (currentUser.roleId.Equals(currentUser.projectManagerRoleId))
            {
                var result = await _projectEntityService.UpdateProjectEntity(projectEntityDTO, id, currentUser);
                return Ok(result);
            }
            return StatusCode((int)HttpStatusCode.Forbidden, "Only user with role PROJECT_MANAGER can perform this action!!!");
        }

        //UPDATE PRIORITY
        [HttpPut]
        [Route("priority")]
        [Authorize(Roles = "PROJECT_MANAGER")]
        public async Task<IActionResult> UpdateProjectEntityPriority([FromBody] List<ProjectEntityUpdateDTO> idList)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);

            //PROJECT_MANAGER
            if (currentUser.roleId.Equals(currentUser.projectManagerRoleId))
            {
                var result = await _projectEntityService.UpdateProjectEntityPriority(idList, currentUser);
                return Ok(result);
            }
            return StatusCode((int)HttpStatusCode.Forbidden, "Only user with role PROJECT_MANAGER can perform this action!!!");
        }

        //DELETE
        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = "PROJECT_MANAGER")]
        public async Task<IActionResult> DeleteProjectEntity(Guid id)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);

            //PROJECT_MANAGER
            if (currentUser.roleId.Equals(currentUser.projectManagerRoleId))
            {
                var result = await _projectEntityService.DeleteProjectEntityById(id);
                return Ok(result);
            }
            return StatusCode((int)HttpStatusCode.Forbidden, "Only user with role PROJECT_MANAGER can perform this action!!!");
        }
    }
}
