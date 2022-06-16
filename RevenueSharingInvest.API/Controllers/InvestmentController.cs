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
    [Route("api/v1.0/investments")]
    [EnableCors]
    //[Authorize]
    public class InvestmentController : ControllerBase
    {
        private readonly IInvestmentService _investmentService;
        private readonly IHttpContextAccessor httpContextAccessor;
        public InvestmentController(IInvestmentService investmentService, IHttpContextAccessor httpContextAccessor)
        {
            _investmentService = investmentService;
            this.httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        public async Task<IActionResult> CreateInvestment([FromBody] InvestmentDTO investmentDTO)
        {
            var result = await _investmentService.CreateInvestment(investmentDTO);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllInvestments(int pageIndex, int pageSize)
        {
            var result = new List<InvestmentDTO>();
            result = await _investmentService.GetAllInvestments(pageIndex, pageSize);
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetInvestmentById(Guid id)
        {
            InvestmentDTO dto = new InvestmentDTO();
            dto = await _investmentService.GetInvestmentById(id);
            return Ok(dto);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateInvestment([FromBody] InvestmentDTO investmentDTO, Guid id)
        {
            var result = await _investmentService.UpdateInvestment(investmentDTO, id);
            return Ok(result);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteInvestment(Guid id)
        {
            var result = await _investmentService.DeleteInvestmentById(id);
            return Ok(result);
        }
    }
}
