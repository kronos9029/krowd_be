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
    [Route("api/v1.0/riskTypes")]
    [EnableCors]
    //[Authorize]
    public class RiskTypeController : ControllerBase
    {
        private readonly IRiskTypeService _riskTypeService;
        private readonly IHttpContextAccessor httpContextAccessor;
        public RiskTypeController(IRiskTypeService riskTypeService, IHttpContextAccessor httpContextAccessor)
        {
            _riskTypeService = riskTypeService;
            this.httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRiskType([FromBody] RiskTypeDTO riskTypeDTO)
        {
            var result = await _riskTypeService.CreateRiskType(riskTypeDTO);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRiskTypes(int pageIndex, int pageSize)
        {
            var result = new List<RiskTypeDTO>();
            result = await _riskTypeService.GetAllRiskTypes(pageIndex, pageSize);
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetRiskTypeById(Guid id)
        {
            RiskTypeDTO dto = new RiskTypeDTO();
            dto = await _riskTypeService.GetRiskTypeById(id);
            return Ok(dto);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateRiskType([FromBody] RiskTypeDTO riskTypeDTO, Guid id)
        {
            var result = await _riskTypeService.UpdateRiskType(riskTypeDTO, id);
            return Ok(result);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteRiskType(Guid id)
        {
            var result = await _riskTypeService.DeleteRiskTypeById(id);
            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> ClearAllRiskTypeData()
        {
            var result = await _riskTypeService.ClearAllRiskTypeData();
            return Ok(result);
        }
    }
}
