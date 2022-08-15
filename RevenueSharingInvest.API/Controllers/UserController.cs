using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RevenueSharingInvest.Business.Models.Constant;
using RevenueSharingInvest.Business.Services;
using RevenueSharingInvest.Data.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
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
        
        public UserController(IUserService userService, IRoleService roleService, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _roleService = roleService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDTO userDTO)
        {
            var result = await _userService.CreateUser(userDTO);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers(int pageIndex, int pageSize, string businessId, string role, string status, RoleEnum temp_field_role)
        {

            var result = new AllUserDTO();
            result = await _userService.GetAllUsers(pageIndex, pageSize, businessId, role, status, temp_field_role.ToString());
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            UserDTO dto = new GetUserDTO();
            dto = await _userService.GetUserById(id);
            return Ok(dto);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateUser([FromForm] UpdateUserDTO userDTO, Guid id)
        {
            var result = await _userService.UpdateUser(userDTO, id);
            return Ok(result);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var result = await _userService.DeleteUserById(id);
            return Ok(result);
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
            if (userDTO != null)
            {
                if (userDTO.business == null)
                {
                    currentUser.roleId = "";
                    currentUser.businessId = "";

                }
                else
                {
                    currentUser.roleId = userDTO.role.id;
                    currentUser.businessId = userDTO.business.id;
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
