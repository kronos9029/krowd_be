using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RevenueSharingInvest.Business.Services;
using RevenueSharingInvest.Data.Models.DTOs;
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

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetInvestorById(String id)
        {
            //string currentCompanyId = httpContextAccessor.HttpContext.User.Claims.First(c => c.Type == ClaimTypes.SerialNumber).Value;
            //string role = httpContextAccessor.HttpContext.User.Claims.First(c => c.Type == ClaimTypes.Role).Value;
            //CompanyDTO dto = new CompanyDTO();
            //if ((role == "Company" && currentCompanyId == id) || role == "Admin" || role == "Worker")
            //{
            InvestorDTO dto = new InvestorDTO();
            dto = await _investorService.GetInvestorById(id);
            return Ok(dto);
            //return StatusCode((int)HttpStatusCode.Forbidden, "You Don't Have Permission To Do This");

        }
    }
}
