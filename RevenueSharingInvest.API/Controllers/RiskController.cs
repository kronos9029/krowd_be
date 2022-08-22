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
    [Route("api/v1.0/risks")]
    [EnableCors]
    public class RiskController : ControllerBase
    {
        private readonly IRiskService _riskService;
        private readonly IHttpContextAccessor httpContextAccessor;
        public RiskController(IRiskService riskService, IHttpContextAccessor httpContextAccessor)
        {
            _riskService = riskService;
            this.httpContextAccessor = httpContextAccessor;
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
            var result = await _riskService.UpdateRisk(riskDTO, id);
            return Ok(result);
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = "PROJECT_MANAGER")]
        public async Task<IActionResult> DeleteRisk(Guid id)
        {
            var result = await _riskService.DeleteRiskById(id);
            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> ClearAllRiskData()
        {
            var result = await _riskService.ClearAllRiskData();
            return Ok(result);
        }
    }
}
