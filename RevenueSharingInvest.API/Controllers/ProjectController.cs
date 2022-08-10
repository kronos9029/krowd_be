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
        private readonly IRoleService _roleService;
        private readonly IBusinessService _businessService;
        private readonly IUserService _userService;
        public ProjectController(IProjectService projectService, 
            IHttpContextAccessor httpContextAccessor, 
            IAuthenticateService authenticateService,
            IRoleService roleService,
            IBusinessService businessService,
            IUserService userService)
        {
            _projectService = projectService;
            _httpContextAccessor = httpContextAccessor;
            _authenticateService = authenticateService;
            _roleService = roleService;
            _businessService = businessService;
            _userService = userService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateProject([FromForm] CreateUpdateProjectDTO projectDTO)
        {
            ThisUserObj currentUser = await GetThisUserInfo(HttpContext);

            if (currentUser.roleId.Equals(currentUser.projectManagerRoleId) 
                && projectDTO.businessId.Equals(currentUser.businessId) 
                && projectDTO.managerId.Equals(currentUser.userId))
            {
                var result = await _projectService.CreateProject(projectDTO);
                return Ok(result);
            }


            return StatusCode((int)HttpStatusCode.Forbidden, "You Don't Have Permission Perform This Action!!");
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProjects(int pageIndex, int pageSize, string businessId, string managerId)
        {
            ThisUserObj currentUser = await GetThisUserInfo(HttpContext);

            RoleDTO roleDTO = await _roleService.GetRoleById(Guid.Parse(currentUser.roleId));
            if(roleDTO != null)
            {
                if (roleDTO.name.Equals(RoleEnum.ADMIN.ToString()))
                {
                    var result = new AllProjectDTO();
                    result = await _projectService.GetAllProjects(pageIndex, pageSize, businessId, managerId, roleDTO.name);


                    return Ok(result);
                }
                else if (roleDTO.name.Equals(RoleEnum.INVESTOR.ToString()))
                {

                }else if (roleDTO.name.Equals(RoleEnum.PROJECT_OWNER.ToString()))
                {
                    var result = new AllProjectDTO();
                    result = await _projectService.GetAllProjects(pageIndex, pageSize, currentUser.businessId, currentUser.userId, roleDTO.name);


                    return Ok(result);
                }
                else if (roleDTO.name.Equals(RoleEnum.BUSINESS_MANAGER.ToString()))
                {
                    var result = new AllProjectDTO();
                    result = await _projectService.GetAllProjects(pageIndex, pageSize, currentUser.businessId, currentUser.userId, roleDTO.name);


                    return Ok(result);
                }

            }
            return StatusCode((int)HttpStatusCode.Forbidden, "You Don't Have Permission Perform This Action!!");
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetProjectById(Guid id)
        {
/*            ThisUserObj userObj = await GetThisUserInfo(HttpContext);

            RoleDTO roleDTO = await _roleService.GetRoleById(Guid.Parse(userObj.roleId));
            if (roleDTO.name.Equals(RoleEnum.))
            {

            }*/
            GetProjectDTO dto = new GetProjectDTO();
            dto = await _projectService.GetProjectById(id);
            return Ok(dto);
        }

        [HttpPut]
        [Route("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateProject([FromForm] CreateUpdateProjectDTO projectDTO, Guid id)
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
        [Authorize]
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
        [Authorize]
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

        private async Task<ThisUserObj> GetThisUserInfo(HttpContext httpContext)
        {
            ThisUserObj currentUser = new();

            currentUser.userId = httpContext.User.Claims.First(c => c.Type == ClaimTypes.SerialNumber).Value;
            currentUser.email = httpContext.User.Claims.First(c => c.Type == "email").Value;
            currentUser.investorId = httpContext.User.Claims.FirstOrDefault(c => c.Type == "investorId").Value;

            List<RoleDTO> roleList = await _roleService.GetAllRoles();
            GetUserDTO userDTO = await _userService.GetUserByEmail(currentUser.email);

            currentUser.roleId = userDTO.role.id;
            currentUser.businessId = userDTO.business.id;

            foreach (RoleDTO role in roleList)
            {
                if (role.name.Equals(Enum.GetNames(typeof(RoleEnum)).ElementAt(0)))
                {
                    currentUser.adminRoleId = role.id;
                }
                if (role.name.Equals(Enum.GetNames(typeof(RoleEnum)).ElementAt(1)))
                {
                    currentUser.investorRoleId = role.id;
                }
                if (role.name.Equals(Enum.GetNames(typeof(RoleEnum)).ElementAt(2)))
                {
                    currentUser.businessManagerRoleId = role.id;
                }
                if (role.name.Equals(Enum.GetNames(typeof(RoleEnum)).ElementAt(3)))
                {
                    currentUser.projectManagerRoleId = role.id;
                }
            }

            return currentUser;

        }
    }
}
