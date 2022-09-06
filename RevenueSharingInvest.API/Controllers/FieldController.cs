using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RevenueSharingInvest.Business.Models.Constant;
using RevenueSharingInvest.Business.Services;
using RevenueSharingInvest.Business.Services.Impls;
using RevenueSharingInvest.Data.Models.DTOs;
using RevenueSharingInvest.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RevenueSharingInvest.API.Controllers
{
    [ApiController]
    [Route("api/v1.0/fields")]
    [EnableCors]
    public class FieldController : ControllerBase
    {
        private readonly IFieldService _fieldService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IRoleService _roleService;
        private readonly IUserService _userService;

        public FieldController(IFieldService fieldService, 
            IHttpContextAccessor httpContextAccessor,
            IRoleService roleService,
            IUserService userService)
        {
            _fieldService = fieldService;
            this.httpContextAccessor = httpContextAccessor;
            _roleService = roleService;
            _userService = userService;
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> CreateField([FromBody] FieldDTO fieldDTO)
        {
            var result = await _fieldService.CreateField(fieldDTO);
            return Ok(result);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllFields(int pageIndex, int pageSize)
        {

            ThisUserObj currentUser = await GetThisUserInfo(HttpContext);

            if(!currentUser.roleId.Equals(currentUser.businessManagerRoleId) && !currentUser.roleId.Equals(currentUser.projectManagerRoleId))
            {
                var result = await _fieldService.GetAllFields(pageIndex, pageSize);
                return Ok(result);
            } else
            {
                var result = await _fieldService.GetFieldsByBusinessId(Guid.Parse(currentUser.businessId));
                return Ok(result);
            }
        }

        [HttpGet]
        [Route("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetFieldById(Guid id)
        {
            var dto = await _fieldService.GetFieldById(id);
            return Ok(dto);
        }

        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> UpdateField([FromBody] FieldDTO fieldDTO, Guid id)
        {

            var result = await _fieldService.UpdateField(fieldDTO, id);
            return Ok(result);
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteField(Guid id)
        {
            var result = await _fieldService.DeleteFieldById(id);
            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> ClearAllFieldData()
        {
            var result = await _fieldService.ClearAllFieldData();
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
