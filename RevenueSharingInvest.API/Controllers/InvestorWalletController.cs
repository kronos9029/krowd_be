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
    [Route("api/v1.0/investorWallets")]
    [EnableCors]
    //[Authorize]
    public class InvestorWalletController : ControllerBase
    {
        private readonly IInvestorWalletService _investorWalletService;
        private readonly IHttpContextAccessor httpContextAccessor;
        public InvestorWalletController(IInvestorWalletService investorWalletService, IHttpContextAccessor httpContextAccessor)
        {
            _investorWalletService = investorWalletService;
            this.httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        public async Task<IActionResult> CreateInvestorWallet([FromBody] InvestorWalletDTO investorWalletDTO)
        {
            var result = await _investorWalletService.CreateInvestorWallet(investorWalletDTO);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllInvestorWallets()
        {
            var result = new List<InvestorWalletDTO>();
            result = await _investorWalletService.GetAllInvestorWallets();
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetInvestorWalletById(Guid id)
        {
            InvestorWalletDTO dto = new InvestorWalletDTO();
            dto = await _investorWalletService.GetInvestorWalletById(id);
            return Ok(dto);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateInvestorWallet([FromBody] InvestorWalletDTO investorWalletDTO, [FromQuery] Guid investorWalletId)
        {
            var result = await _investorWalletService.UpdateInvestorWallet(investorWalletDTO, investorWalletId);
            return Ok(result);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteInvestorWallet(Guid id)
        {
            var result = await _investorWalletService.DeleteInvestorWalletById(id);
            return Ok(result);
        }
    }
}