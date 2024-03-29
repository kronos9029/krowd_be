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
    [Route("api/v1.0/investor_types")]
    [EnableCors]
    //[Authorize]
    public class InvestorTypeController : ControllerBase
    {
        private readonly IInvestorTypeService _investorTypeService;
        private readonly IHttpContextAccessor httpContextAccessor;
        public InvestorTypeController(IInvestorTypeService investorTypeService, IHttpContextAccessor httpContextAccessor)
        {
            _investorTypeService = investorTypeService;
            this.httpContextAccessor = httpContextAccessor;
        }

        //CREATE
        [HttpPost]
        public async Task<IActionResult> CreateInvestorType([FromBody] InvestorTypeDTO investorTypeDTO)
        {
            var result = await _investorTypeService.CreateInvestorType(investorTypeDTO);
            return Ok(result);
        }

        //GET ALL
        [HttpGet]
        public async Task<IActionResult> GetAllInvestorTypes(int pageIndex, int pageSize)
        {
            var result = new List<GetInvestorTypeDTO>();
            result = await _investorTypeService.GetAllInvestorTypes(pageIndex, pageSize);
            return Ok(result);
        }

        //GET BY ID
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetInvestorTypeById(Guid id)
        {
            GetInvestorTypeDTO dto = new GetInvestorTypeDTO();
            dto = await _investorTypeService.GetInvestorTypeById(id);
            return Ok(dto);
        }

        //UPDATE
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateInvestorType([FromBody] InvestorTypeDTO investorTypeDTO, Guid id)
        {
            var result = await _investorTypeService.UpdateInvestorType(investorTypeDTO, id);
            return Ok(result);
        }

        //DELETE
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteInvestorType(Guid id)
        {
            var result = await _investorTypeService.DeleteInvestorTypeById(id);
            return Ok(result);
        }
    }
}
