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
    [Route("api/v1.0/business_fields")]
    [EnableCors]
    //[Authorize]
    public class BusinessFieldController : ControllerBase
    {
        private readonly IBusinessFieldService _businessFieldService;
        private readonly IHttpContextAccessor httpContextAccessor;
        public BusinessFieldController(IBusinessFieldService businessFieldService, IHttpContextAccessor httpContextAccessor)
        {
            _businessFieldService = businessFieldService;
            this.httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBusinessField([FromBody] BusinessFieldDTO businessFieldDTO)
        {
            var result = await _businessFieldService.CreateBusinessField(businessFieldDTO);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBusinessFields(int pageIndex, int pageSize)
        {
            var result = new List<BusinessFieldDTO>();
            result = await _businessFieldService.GetAllBusinessFields(pageIndex, pageSize);
            return Ok(result);
        }

        [HttpGet]
        [Route("{business_id},{field_id}")]
        public async Task<IActionResult> GetBusinessFieldById(Guid business_id, Guid field_id)
        {
            BusinessFieldDTO dto = new BusinessFieldDTO();
            dto = await _businessFieldService.GetBusinessFieldById(business_id, field_id);
            return Ok(dto);
        }

        [HttpPut]
        [Route("{business_id},{field_id}")]
        public async Task<IActionResult> UpdateBusinessField([FromBody] BusinessFieldDTO businessFieldDTO, Guid business_id, Guid field_id)
        {
            var result = await _businessFieldService.UpdateBusinessField(businessFieldDTO, business_id, field_id);
            return Ok(result);
        }

        [HttpDelete]
        [Route("{business_id},{field_id}")]
        public async Task<IActionResult> DeleteBusinessField(Guid business_id, Guid field_id)
        {
            var result = await _businessFieldService.DeleteBusinessFieldById(business_id, field_id);
            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> ClearAllBusinessFieldData()
        {
            var result = await _businessFieldService.ClearAllBusinessFieldData();
            return Ok(result);
        }
    }
}
