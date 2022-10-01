using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RevenueSharingInvest.API.Extensions;
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

            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);

            if(!currentUser.roleId.Equals(currentUser.businessManagerRoleId) && !currentUser.roleId.Equals(currentUser.projectManagerRoleId))
            {
                var result = await _fieldService.GetAllFields(pageIndex, pageSize);
                return Ok(result);
            } else if ((currentUser.roleId.Equals(currentUser.businessManagerRoleId) || currentUser.roleId.Equals(currentUser.projectManagerRoleId)) && currentUser.businessId.Equals(""))
            {
                var result = await _fieldService.GetAllFields(0, 0);
                return Ok(result);
            }
            else
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
    }
}
