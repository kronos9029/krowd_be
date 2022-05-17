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
    [Route("api/v1.0/periodRevenues")]
    [EnableCors]
    //[Authorize]
    public class PeriodRevenueController : ControllerBase
    {
        private readonly IPeriodRevenueService _periodRevenueService;
        private readonly IHttpContextAccessor httpContextAccessor;
        public PeriodRevenueController(IPeriodRevenueService periodRevenueService, IHttpContextAccessor httpContextAccessor)
        {
            _periodRevenueService = periodRevenueService;
            this.httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePeriodRevenue([FromBody] PeriodRevenueDTO periodRevenueDTO)
        {
            var result = await _periodRevenueService.CreatePeriodRevenue(periodRevenueDTO);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPeriodRevenues()
        {
            var result = new List<PeriodRevenueDTO>();
            result = await _periodRevenueService.GetAllPeriodRevenues();
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetPeriodRevenueById(Guid id)
        {
            PeriodRevenueDTO dto = new PeriodRevenueDTO();
            dto = await _periodRevenueService.GetPeriodRevenueById(id);
            return Ok(dto);
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePeriodRevenue([FromBody] PeriodRevenueDTO periodRevenueDTO, [FromQuery] Guid periodRevenueId)
        {
            var result = await _periodRevenueService.UpdatePeriodRevenue(periodRevenueDTO, periodRevenueId);
            return Ok(result);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeletePeriodRevenue(Guid id)
        {
            var result = await _periodRevenueService.DeletePeriodRevenueById(id);
            return Ok(result);
        }
    }
}
