using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RevenueSharingInvest.Business.Models.Constant;
using RevenueSharingInvest.Business.Services;
using RevenueSharingInvest.Data.Helpers.Logger;
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
    [Route("api/v1.0/areas")]
    [EnableCors]
    //[Authorize]
    public class AreaController : ControllerBase
    {
        private readonly IAreaService _areaService;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthenticateService _authenticateService;
        public AreaController(IAreaService areaService, IUserService userService, IRoleService roleService, IHttpContextAccessor httpContextAccessor, IAuthenticateService authenticateService)
        {
            _areaService = areaService;
            _userService = userService;
            _roleService = roleService;
            _httpContextAccessor = httpContextAccessor;
            _authenticateService = authenticateService;
        }

        //CREATE
        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> CreateArea([FromBody] CreateUpdateAreaDTO areaDTO)
        {
            ThisUserObj currentUser = await GetThisUserInfo(HttpContext);

            //ADMIN
            if (currentUser.roleId.Equals(currentUser.adminRoleId))
            {
                var result = await _areaService.CreateArea(areaDTO, currentUser);
                return Ok(result);
            }
            return StatusCode((int)HttpStatusCode.Forbidden, "Only user with role ADMIN can perform this action!!!");
        }

        //GET ALL
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllAreas(int pageIndex, int pageSize)
        {
            ThisUserObj currentUser = await GetThisUserInfo(HttpContext);

            //ALL ROLES
            if (currentUser.roleId.Equals(currentUser.adminRoleId)
                || currentUser.roleId.Equals(currentUser.businessManagerRoleId)
                || currentUser.roleId.Equals(currentUser.projectManagerRoleId)
                || currentUser.roleId.Equals(currentUser.investorRoleId)
                || currentUser.roleId.Equals(""))
            {
                var result = new List<GetAreaDTO>();
                result = await _areaService.GetAllAreas(pageIndex, pageSize, currentUser);
                return Ok(result);
            }

            return StatusCode((int)HttpStatusCode.Forbidden, "You Don't Have Permission Perform This Action!!");
        }

        //GET BY ID
        [HttpGet]
        [Route("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAreaById(Guid id)
        {
            ThisUserObj currentUser = await GetThisUserInfo(HttpContext);

            //ALL ROLES
            if (currentUser.roleId.Equals(currentUser.adminRoleId)
                || currentUser.roleId.Equals(currentUser.businessManagerRoleId)
                || currentUser.roleId.Equals(currentUser.projectManagerRoleId)
                || currentUser.roleId.Equals(currentUser.investorRoleId)
                || currentUser.roleId.Equals(""))
            {
                var result = new GetAreaDTO();
                result = await _areaService.GetAreaById(id, currentUser);
                return Ok(result);
            }

            return StatusCode((int)HttpStatusCode.Forbidden, "You Don't Have Permission Perform This Action!!");
        }

        //UPDATE
        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> UpdateArea([FromBody] CreateUpdateAreaDTO areaDTO, Guid id)
        {
            ThisUserObj currentUser = await GetThisUserInfo(HttpContext);

            //ADMIN
            if (currentUser.roleId.Equals(currentUser.adminRoleId))
            {
                var result = await _areaService.UpdateArea(areaDTO, id, currentUser);
                return Ok(result);
            }
            return StatusCode((int)HttpStatusCode.Forbidden, "Only user with role ADMIN can perform this action!!!");
        }

        //DELETE
        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteArea(Guid id)
        {
            ThisUserObj currentUser = await GetThisUserInfo(HttpContext);

            //ADMIN
            if (currentUser.roleId.Equals(currentUser.adminRoleId))
            {
                var result = await _areaService.DeleteAreaById(id, currentUser);
                return Ok(result);
            }
            return StatusCode((int)HttpStatusCode.Forbidden, "Only user with role ADMIN can perform this action!!!");
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
