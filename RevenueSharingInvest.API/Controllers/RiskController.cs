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

        //CREATE
        [HttpPost]
        [Authorize(Roles = "PROJECT_MANAGER")]
        public async Task<IActionResult> CreateRisk([FromBody] RiskDTO riskDTO)
        {
            var result = await _riskService.CreateRisk(riskDTO);
            return Ok(result);
        }

        //GET ALL
        [HttpGet]
        [Authorize(Roles = "PROJECT_MANAGER")]
        public async Task<IActionResult> GetAllRisks(int pageIndex, int pageSize)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);

            var result = new List<RiskDTO>();
            result = await _riskService.GetAllRisksByBusinessId(pageIndex, pageSize, currentUser.businessId);
            return Ok(result);
        }

        //GET BY ID
        [HttpGet]
        [Route("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRiskById(Guid id)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);

            RiskDTO dto = new RiskDTO();
            dto = await _riskService.GetRiskById(id, currentUser);
            return Ok(dto);
        }

        //UPDATE
        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = "PROJECT_MANAGER")]
        public async Task<IActionResult> UpdateRisk([FromBody] RiskDTO riskDTO, Guid id)
        {

            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);

            var result = await _riskService.UpdateRisk(riskDTO, id, currentUser);
            return Ok(result);
        }

        //DELETE
        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = "PROJECT_MANAGER")]
        public async Task<IActionResult> DeleteRisk(Guid id)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);
            var result = await _riskService.DeleteRiskById(id, currentUser);
            return Ok(result);
        }
    }
}
