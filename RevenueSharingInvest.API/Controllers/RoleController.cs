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
using System.Threading.Tasks;

namespace RevenueSharingInvest.API.Controllers
{
    [ApiController]
    [Route("api/v1.0/roles")]
    [EnableCors]
    //[Authorize]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly IHttpContextAccessor httpContextAccessor;
        public RoleController(IRoleService roleService, IHttpContextAccessor httpContextAccessor)
        {
            _roleService = roleService;
            this.httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] RoleDTO roleDTO)
        {
            var result = await _roleService.CreateRole(roleDTO);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRoles()
        {
            var result = new List<RoleDTO>();
            result = await _roleService.GetAllRoles();
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetRoleById(Guid id)
        {
            RoleDTO dto = new RoleDTO();
            dto = await _roleService.GetRoleById(id);
            return Ok(dto);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateRole([FromBody] RoleDTO roleDTO, Guid id)
        {
            var result = await _roleService.UpdateRole(roleDTO, id);
            return Ok(result);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteRole(Guid id)
        {
            var result = await _roleService.DeleteRoleById(id);
            return Ok(result);
        }
    }
}
