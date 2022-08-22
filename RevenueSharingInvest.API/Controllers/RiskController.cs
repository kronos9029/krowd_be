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
    [Route("api/v1.0/risks")]
    [EnableCors]
    public class RiskController : ControllerBase
    {
        private readonly IRiskService _riskService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRoleService _roleService;
        private readonly IUserService _userService;

        public RiskController(IRiskService riskService, 
            IHttpContextAccessor httpContextAccessor,
            IRoleService roleService,
            IUserService userService)
        {
            _riskService = riskService;
            _httpContextAccessor = httpContextAccessor;
            _roleService = roleService;
            _userService = userService;
        }

        [HttpPost]
        [Authorize(Roles = "PROJECT_MANAGER")]
        public async Task<IActionResult> CreateRisk([FromBody] RiskDTO riskDTO)
        {
            var result = await _riskService.CreateRisk(riskDTO);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRisks(int pageIndex, int pageSize)
        {
            var result = new List<RiskDTO>();
            result = await _riskService.GetAllRisks(pageIndex, pageSize);
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRiskById(Guid id)
        {
            RiskDTO dto = new RiskDTO();
            dto = await _riskService.GetRiskById(id);
            return Ok(dto);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateRisk([FromBody] RiskDTO riskDTO, Guid id)
        {

            ThisUserObj currentUser = await GetThisUserInfo(HttpContext);

            var result = await _riskService.UpdateRisk(riskDTO, id, currentUser);
            return Ok(result);
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = "PROJECT_MANAGER")]
        public async Task<IActionResult> DeleteRisk(Guid id)
        {
            ThisUserObj currentUser = await GetThisUserInfo(HttpContext);
            var result = await _riskService.DeleteRiskById(id, currentUser);
            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> ClearAllRiskData()
        {
            var result = await _riskService.ClearAllRiskData();
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
