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
    [Route("api/v1.0/packages")]
    [EnableCors]
    //[Authorize]
    public class PackageController : ControllerBase
    {
        private readonly IPackageService _packageService;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IHttpContextAccessor httpContextAccessor;
        public PackageController(IPackageService packageService, IUserService userService, IRoleService roleService, IHttpContextAccessor httpContextAccessor)
        {
            _packageService = packageService;
            _userService = userService;
            _roleService = roleService;
            this.httpContextAccessor = httpContextAccessor;
        }

        //CREATE
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreatePackage([FromForm] CreateUpdatePackageDTO packageDTO)
        {
            ThisUserObj currentUser = await GetThisUserInfo(HttpContext);

            //PROJECT_MANAGER
            if (currentUser.roleId.Equals(currentUser.projectManagerRoleId))
            {
                var result = await _packageService.CreatePackage(packageDTO, currentUser);
                return Ok(result);
            }
            return StatusCode((int)HttpStatusCode.Forbidden, "Only user with role PROJECT_MANAGER can perform this action!!!");
        }


        //GET BY PROJECT_ID
        [HttpGet]
        [Route("project/{project_id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllPackagesByProjectId(string project_id)
        {
            ThisUserObj currentUser = await GetThisUserInfo(HttpContext);

            //ALL ROLES
            if (currentUser.roleId.Equals(currentUser.adminRoleId)
                || currentUser.roleId.Equals(currentUser.businessManagerRoleId)
                || currentUser.roleId.Equals(currentUser.projectManagerRoleId)
                || currentUser.roleId.Equals(currentUser.investorRoleId)
                || currentUser.roleId.Equals(""))
            {
                var result = new AllProjectPackageDTO();
                result = await _packageService.GetAllPackagesByProjectId(project_id, currentUser);
                return Ok(result);
            }

            return StatusCode((int)HttpStatusCode.Forbidden, "You Don't Have Permission Perform This Action!!");
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetPackageById(Guid id)
        {
            ThisUserObj currentUser = await GetThisUserInfo(HttpContext);

            if (currentUser.roleId.Equals(currentUser.adminRoleId)
                || currentUser.roleId.Equals(currentUser.businessManagerRoleId)
                || currentUser.roleId.Equals(currentUser.projectManagerRoleId)
                || currentUser.roleId.Equals(currentUser.investorRoleId)
                || currentUser.roleId.Equals(""))
            {
                GetPackageDTO dto = new GetPackageDTO();
                dto = await _packageService.GetPackageById(id, currentUser);
                return Ok(dto);
            }

            return StatusCode((int)HttpStatusCode.Forbidden, "You Don't Have Permission Perform This Action!!");
        }

        [HttpPut]
        [Route("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdatePackage([FromForm] CreateUpdatePackageDTO packageDTO, Guid id)
        {
            ThisUserObj currentUser = await GetThisUserInfo(HttpContext);

            //PROJECT_MANAGER
            if (currentUser.roleId.Equals(currentUser.projectManagerRoleId))
            {
                var result = await _packageService.UpdatePackage(packageDTO, id, currentUser);
                return Ok(result);
            }
            return StatusCode((int)HttpStatusCode.Forbidden, "Only user with role PROJECT_MANAGER can perform this action!!!");
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize]
        public async Task<IActionResult> DeletePackage(Guid id)
        {
            ThisUserObj currentUser = await GetThisUserInfo(HttpContext);

            //PROJECT_MANAGER
            if (currentUser.roleId.Equals(currentUser.projectManagerRoleId))
            {
                var result = await _packageService.DeletePackageById(id);
                return Ok(result);
            }
            return StatusCode((int)HttpStatusCode.Forbidden, "Only user with role PROJECT_MANAGER can perform this action!!!");
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
