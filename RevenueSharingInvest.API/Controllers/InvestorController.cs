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
    [Route("api/v1.0/investors")]
    [EnableCors]
    //[Authorize]
    public class InvestorController : ControllerBase
    {
        private readonly IInvestorService _investorService;
        private readonly IHttpContextAccessor httpContextAccessor;
        public InvestorController(IInvestorService investorService, IHttpContextAccessor httpContextAccessor)
        {
            _investorService = investorService;
            this.httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        public async Task<IActionResult> CreateInvestor([FromBody] InvestorDTO investorDTO)
        {
            var result = await _investorService.CreateInvestor(investorDTO);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllInvestors(int pageIndex, int pageSize)
        {
            var result = new List<InvestorDTO>();
            result = await _investorService.GetAllInvestors(pageIndex, pageSize);
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetInvestorById(Guid id)
        {
            InvestorDTO dto = new InvestorDTO();
            dto = await _investorService.GetInvestorById(id);
            return Ok(dto);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateInvestor([FromBody] InvestorDTO investorDTO, Guid id)
        {
            var result = await _investorService.UpdateInvestor(investorDTO, id);
            return Ok(result);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteInvestor(Guid id)
        {
            var result = await _investorService.DeleteInvestorById(id);
            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> ClearAllInvestorData()
        {
            var result = await _investorService.ClearAllInvestorData();
            return Ok(result);
        }
    }
}
