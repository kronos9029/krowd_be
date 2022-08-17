using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RevenueSharingInvest.Business.Exceptions;
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
    [Route("api/v1.0/users")]
    [EnableCors]
    //[Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserController(IUserService userService, IHttpContextAccessor httpContextAccessor, IRoleService roleService)
        {
            _userService = userService;
            _roleService = roleService;
            _httpContextAccessor = httpContextAccessor;
            _roleService = roleService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDTO userDTO)
        {

            ThisUserObj currentUser = await GetThisUserInfo(HttpContext);

            if (currentUser.roleId.Equals(currentUser.adminRoleId))
            {

                var result = await _userService.CreateUser(userDTO, currentUser.businessId);
                return Ok(result);
            }
            else if (currentUser.roleId.Equals(currentUser.businessManagerRoleId))
            {

                var result = await _userService.CreateUser(userDTO, currentUser.businessId);
                return Ok(result);
            }

            return StatusCode((int)HttpStatusCode.Forbidden, "You Do Not Have Permission To Access This Business!!");

        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers(int pageIndex, int pageSize, string businessId, string role, string status)
        {
            ThisUserObj currentUser = await GetThisUserInfo(HttpContext);
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
            ThisUserObj currentUser = await GetThisUserInfo(HttpContext);
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
        public async Task<IActionResult> UpdateUser([FromForm] UpdateUserDTO userDTO, Guid id)
        {
            ThisUserObj currentUser = await GetThisUserInfo(HttpContext);

            if (currentUser.roleId.Equals(currentUser.adminRoleId))
            {
                var result = await _userService.UpdateUser(userDTO, id);
                return Ok(result);
            }
            else if (currentUser.userId.Equals(id.ToString()))
            {
                var result = await _userService.UpdateUser(userDTO, id);
                return Ok(result);
            }

            return StatusCode((int)HttpStatusCode.Forbidden, "You Do Not Have Permission To Access This Business!!");
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            ThisUserObj currentUser = await GetThisUserInfo(HttpContext);
            GetUserDTO userDTO = await _userService.GetUserById(id);

/*            if (userDTO.role.id.Equals(currentUser.adminRoleId))
            {
            }*/



            if (userDTO != null)
            {
                var result = await _userService.DeleteUserById(id);
                return Ok(result);
            }

            return StatusCode((int)HttpStatusCode.Forbidden, "You Do Not Have Permission To Access This Business!!");
        }

        [HttpDelete]
        public async Task<IActionResult> ClearAllUserData()
        {
            var result = await _userService.ClearAllUserData();
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

        //[HttpPost]
        //[AllowAnonymous]
        //[Route("authenticate-mobile")]
        //public async Task<IActionResult> AuthenticateMobile([FromQuery]string token)
        //{
        //    var result = await _userService.GetTokenInvestor(token);
        //    return Ok(result);
        //}