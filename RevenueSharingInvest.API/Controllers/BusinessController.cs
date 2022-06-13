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
    [Route("api/v1.0/businesses")]
    [EnableCors]
    //[Authorize]
    public class BusinessController : ControllerBase
    {
        private readonly IBusinessService _businessService;
        private readonly IHttpContextAccessor httpContextAccessor;
        public BusinessController(IBusinessService businessService, IHttpContextAccessor httpContextAccessor)
        {
            _businessService = businessService;
            this.httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBusiness([FromBody] BusinessDTO businessDTO)
        {
            var result = await _businessService.CreateBusiness(businessDTO);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBusinesses(int pageIndex, int pageSize)
        {
            var result = new List<BusinessDTO>();
            result = await _businessService.GetAllBusiness(pageIndex, pageSize);
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetBusinessById(Guid id)
        {
            BusinessDTO dto = new BusinessDTO();
            dto = await _businessService.GetBusinessById(id);
            return Ok(dto);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateBusiness([FromBody] BusinessDTO businessDTO, Guid id)
        {
            var result = await _businessService.UpdateBusiness(businessDTO, id);
            return Ok(result);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteBusiness(Guid id)
        {
            var result = await _businessService.DeleteBusinessById(id);
            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> ClearAllBusinessData()
        {
            var result = await _businessService.ClearAllBusinessData();
            return Ok(result);
        }
    }
}
