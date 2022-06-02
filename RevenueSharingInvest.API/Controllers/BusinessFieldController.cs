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
        [Route("{businessId},{fieldId}")]
        public async Task<IActionResult> GetBusinessFieldById(Guid businessId, Guid fieldId)
        {
            BusinessFieldDTO dto = new BusinessFieldDTO();
            dto = await _businessFieldService.GetBusinessFieldById(businessId, fieldId);
            return Ok(dto);
        }

        [HttpPut]
        [Route("{businessId},{fieldId}")]
        public async Task<IActionResult> UpdateBusinessField([FromBody] BusinessFieldDTO businessFieldDTO, Guid businessId, Guid fieldId)
        {
            var result = await _businessFieldService.UpdateBusinessField(businessFieldDTO, businessId, fieldId);
            return Ok(result);
        }

        [HttpDelete]
        [Route("{businessId},{fieldId}")]
        public async Task<IActionResult> DeleteBusinessField(Guid businessId, Guid fieldId)
        {
            var result = await _businessFieldService.DeleteBusinessFieldById(businessId, fieldId);
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
