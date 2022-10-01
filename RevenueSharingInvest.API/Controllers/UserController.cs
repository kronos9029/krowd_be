using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RevenueSharingInvest.API.Extensions;
using RevenueSharingInvest.Business.Exceptions;
using RevenueSharingInvest.Business.Models.Constant;
using RevenueSharingInvest.Business.Services;
using RevenueSharingInvest.Data.Models.Constants;
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
    [Route("api/v1.0/users")]
    [EnableCors]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserController(IUserService userService, IRoleService roleService, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _roleService = roleService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDTO userDTO)
        {

            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);

            if (currentUser.roleId.Equals(currentUser.adminRoleId)
                || currentUser.roleId.Equals(currentUser.businessManagerRoleId))
            {
                var result = await _userService.CreateUser(userDTO, currentUser);
                return Ok(result);
            }
            return StatusCode((int)HttpStatusCode.Forbidden, "Only user with role ADMIN or BUSINESS_MANAGER can perform this action!!!");

        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllUsers(int pageIndex, int pageSize, string businessId, string role, string status)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);
            var result = new AllUserDTO();

            if (currentUser.roleId.Equals(currentUser.adminRoleId) || currentUser.roleId.Equals(currentUser.businessManagerRoleId) || currentUser.roleId.Equals(currentUser.projectManagerRoleId))
            {
                result = await _userService.GetAllUsers(pageIndex, pageSize, businessId, role, status, currentUser);
                return Ok(result);
            }
            return StatusCode((int)HttpStatusCode.Forbidden, "Only user with role ADMIN or BUSINESS_MANAGER or PROJECT_MANAGER can perform this action!!!");

        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);
            GetUserDTO dto = new GetUserDTO();

            if (currentUser.roleId.Equals(currentUser.adminRoleId))
            {
                dto = await _userService.GetUserById(id);
                return Ok(dto);
            }
            else if (currentUser.roleId.Equals(currentUser.businessManagerRoleId))
            {
                dto = await _userService.GetUserById(id);
                if ((dto.role.id.Equals(currentUser.projectManagerRoleId) && dto.business.id.Equals(currentUser.businessId)) || currentUser.userId.Equals(id.ToString()))
                {
                    return Ok(dto);
                }
                else
                {
                    dto = null;
                    dto = await _userService.BusinessManagerGetUserById(currentUser.businessId, id);
                    if (dto == null)
                        throw new NotFoundException("You Do Not Have Permission To Access This User");
                    else
                        return Ok(dto);
                }
            }
            else if (currentUser.roleId.Equals(currentUser.projectManagerRoleId))
            {
                dto = await _userService.GetUserById(id);
                if ((dto.role.id.Equals(currentUser.businessManagerRoleId) && dto.business.id.Equals(currentUser.businessId)) 
                    || currentUser.userId.Equals(id.ToString()))
                {
                    return Ok(dto);
                }
                else
                {
                    dto = null;
                    dto = await _userService.ProjectManagerGetUserbyId(currentUser.businessId, id);
                    if (dto == null)
                        throw new NotFoundException("You Do Not Have Permission To Access This User");
                    else
                        return Ok(dto);
                }
            }
            else if (currentUser.roleId.Equals(currentUser.investorRoleId))
            {
                if (currentUser.userId.Equals(id.ToString()))
                {
                    dto = await _userService.GetUserById(id);
                    return Ok(dto);
                }

            }

            return StatusCode((int)HttpStatusCode.Forbidden, "You Do Not Have Permission To Access This Business!!");
        }

        [HttpPut]
        [Route("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser([FromForm] UpdateUserDTO userDTO, Guid id)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);

            if (currentUser.roleId.Equals(currentUser.adminRoleId) 
                || currentUser.roleId.Equals(currentUser.businessManagerRoleId) 
                || currentUser.roleId.Equals(currentUser.projectManagerRoleId) 
                || currentUser.roleId.Equals(currentUser.investorRoleId))
            {
                var result = await _userService.UpdateUser(userDTO, id, currentUser);
                return Ok(result);
            }
            return StatusCode((int)HttpStatusCode.Forbidden, "You Do Not Have Permission To Access This Business!!");
        }

        //UPDATE STATUS
        [HttpPut]
        [Route("status/{id},{status}")]
        [Authorize]
        public async Task<IActionResult> UpdateUserStatus(Guid id, string status)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);

            if (currentUser.roleId.Equals(currentUser.adminRoleId)
                || currentUser.roleId.Equals(currentUser.businessManagerRoleId))
            {
                var result = await _userService.UpdateUserStatus(id, status, currentUser);
                return Ok(result);
            }
            return StatusCode((int)HttpStatusCode.Forbidden, "Only user with role ADMIN or BUSINESS_MANAGER can perform this action!!!!!");
        }

        //UPDATE EMAIL
        [HttpPut]
        [Route("email/{id},{email}")]
        [Authorize]
        public async Task<IActionResult> UpdateUserEmail(Guid id, string email)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);

            if (currentUser.roleId.Equals(currentUser.adminRoleId)
                || currentUser.roleId.Equals(currentUser.businessManagerRoleId))
            {
                var result = await _userService.UpdateUserEmail(id, email, currentUser);
                return Ok();
            }
            return StatusCode((int)HttpStatusCode.Forbidden, "Only user with role ADMIN or BUSINESS_MANAGER can perform this action!!!!!");
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);
            GetUserDTO userDTO = await _userService.GetUserById(id);
            if (userDTO != null)
            {
                throw new NotFoundException("No Such User With This Id!!");
            }
            if (userDTO.role.id.Equals(currentUser.adminRoleId))
            {
                if (userDTO.status.Equals(ObjectStatusEnum.INACTIVE.ToString()) || userDTO.status.Equals(ObjectStatusEnum.BLOCKED.ToString()))
                {
                    var result = await _userService.DeleteUserById(id);
                    return Ok(result);
                }
            }

            return StatusCode((int)HttpStatusCode.Forbidden, "You Do Not Have Permission To Perfrom This Action!!");
        }

        [HttpDelete]
        public async Task<IActionResult> ClearAllUserData()
        {
            var result = await _userService.ClearAllUserData();
            return Ok(result);
        }
    }
}

        //[HttpPost]
        //[AllowAnonymous]
        //[Route("authenticate-mobile")]
        //public async Task<IActionResult> AuthenticateMobile([FromQuery]string token)
        //{
        //    var result = await _userService.GetTokenInvestor(token);
        //    return Ok(result);
        //}