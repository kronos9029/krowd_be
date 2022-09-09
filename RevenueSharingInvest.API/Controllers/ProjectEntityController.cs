using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        //[Authorize(Roles = "PROJECT_MANAGER")]
        public async Task<IActionResult> CreateProjectEntity([FromBody] CreateUpdateProjectEntityDTO projectEntityDTO)
        {
            //ThisUserObj currentUser = await GetThisUserInfo(HttpContext);

            //PROJECT_MANAGER
            //if (currentUser.roleId.Equals(currentUser.projectManagerRoleId))
            //{
                var result = await _projectEntityService.CreateProjectEntity(projectEntityDTO, null);
                return Ok(result);
            //}
            //return StatusCode((int)HttpStatusCode.Forbidden, "Only user with role PROJECT_MANAGER can perform this action!!!");
        }

        [HttpGet]
        [Authorize(Roles = "PROJECT_MANAGER")]
        public async Task<IActionResult> GetAllProjectEntities(int pageIndex, int pageSize)
        {
            var result = new List<GetProjectEntityDTO>();
            result = await _projectEntityService.GetAllProjectEntities(pageIndex, pageSize);
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        [Authorize(Roles = "PROJECT_MANAGER")]
        public async Task<IActionResult> GetProjectEntityById(Guid id)
        {
            GetProjectEntityDTO dto = new GetProjectEntityDTO();
            dto = await _projectEntityService.GetProjectEntityById(id);
            return Ok(dto);
        }

        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = "PROJECT_MANAGER")]
        public async Task<IActionResult> UpdateProjectEntity([FromForm] CreateUpdateProjectEntityDTO projectEntityDTO, Guid id)
        {
            var result = await _projectEntityService.UpdateProjectEntity(projectEntityDTO, id);
            return Ok(result);
        }

        //[HttpPut]
        //public async Task<IActionResult> UpdateProjectEntityPriority([FromQuery] List<string> idList)
        //{
        //    var result = await _projectEntityService.UpdateProjectEntityPriority(idList);
        //    return Ok(result);
        //}

        [HttpPut]
        public async Task<IActionResult> UpdateProjectEntityPriority([FromBody] List<ProjectEntityUpdateDTO> idList)
        {
            var result = await _projectEntityService.UpdateProjectEntityPriority(idList);
            return Ok(result);
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = "PROJECT_MANAGER")]
        public async Task<IActionResult> DeleteProjectEntity(Guid id)
        {
            var result = await _projectEntityService.DeleteProjectEntityById(id);
            return Ok(result);
        }

        private async Task<ThisUserObj> GetThisUserInfo(HttpContext? httpContext)
        {
            ThisUserObj currentUser = new();

            var checkUser = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.SerialNumber);
            if (checkUser == null)
            {
                currentUser.userId = "";
                currentUser.email = "";
                currentUser.investorId = "";
            }
            else
            {
                currentUser.userId = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.SerialNumber).Value;
                currentUser.email = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
                currentUser.investorId = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.GroupSid).Value;
            }

            List<RoleDTO> roleList = await _roleService.GetAllRoles();
            GetUserDTO? userDTO = await _userService.GetUserByEmail(currentUser.email);
            if (userDTO == null)
            {
                currentUser.roleId = "";
                currentUser.businessId = "";

            }
            else
            {
                if (userDTO.business != null)
                {
                    currentUser.roleId = userDTO.role.id;
                    currentUser.businessId = userDTO.business.id;
                }
                else
                {
                    currentUser.roleId = userDTO.role.id;
                    currentUser.businessId = "";
                }
            }

            foreach (RoleDTO role in roleList)
            {
                if (role.name.Equals(Enum.GetNames(typeof(RoleEnum)).ElementAt(0)))
                {
                    currentUser.adminRoleId = role.id;
                }
                if (role.name.Equals(Enum.GetNames(typeof(RoleEnum)).ElementAt(3)))
                {
                    currentUser.investorRoleId = role.id;
                }
                if (role.name.Equals(Enum.GetNames(typeof(RoleEnum)).ElementAt(1)))
                {
                    currentUser.businessManagerRoleId = role.id;
                }
                if (role.name.Equals(Enum.GetNames(typeof(RoleEnum)).ElementAt(2)))
                {
                    currentUser.projectManagerRoleId = role.id;
                }
            }

            return currentUser;
        }
    }
}
