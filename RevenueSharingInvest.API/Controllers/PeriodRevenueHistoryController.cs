﻿using Microsoft.AspNetCore.Authorization;
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
    [Route("api/v1.0/periodRevenueHistories")]
    [EnableCors]
    //[Authorize]
    public class PeriodRevenueHistoryController : ControllerBase
    {
        private readonly IPeriodRevenueHistoryService _periodRevenueHistoryService;
        private readonly IHttpContextAccessor httpContextAccessor;
        public PeriodRevenueHistoryController(IPeriodRevenueHistoryService periodRevenueHistoryService, IHttpContextAccessor httpContextAccessor)
        {
            _periodRevenueHistoryService = periodRevenueHistoryService;
            this.httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePeriodRevenueHistory([FromBody] PeriodRevenueHistoryDTO periodRevenueHistoryDTO)
        {
            var result = await _periodRevenueHistoryService.CreatePeriodRevenueHistory(periodRevenueHistoryDTO);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPeriodRevenueHistorys()
        {
            var result = new List<PeriodRevenueHistoryDTO>();
            result = await _periodRevenueHistoryService.GetAllPeriodRevenueHistorys();
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetPeriodRevenueHistoryById(Guid id)
        {
            PeriodRevenueHistoryDTO dto = new PeriodRevenueHistoryDTO();
            dto = await _periodRevenueHistoryService.GetPeriodRevenueHistoryById(id);
            return Ok(dto);
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePeriodRevenueHistory([FromBody] PeriodRevenueHistoryDTO periodRevenueHistoryDTO, [FromQuery] Guid periodRevenueHistoryId)
        {
            var result = await _periodRevenueHistoryService.UpdatePeriodRevenueHistory(periodRevenueHistoryDTO, periodRevenueHistoryId);
            return Ok(result);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeletePeriodRevenueHistory(Guid id)
        {
            var result = await _periodRevenueHistoryService.DeletePeriodRevenueHistoryById(id);
            return Ok(result);
        }
    }
}
