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
    [Route("api/v1.0/businessFields")]
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
    public async Task<IActionResult> GetAllBusinessFields()
    {
        var result = new List<BusinessFieldDTO>();
        result = await _businessFieldService.GetAllBusinessFields();
        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateBusinessField([FromBody] BusinessFieldDTO businessFieldDTO, [FromQuery] Guid businessFieldId)
    {
        var result = await _businessFieldService.UpdateBusinessField(businessFieldDTO, businessFieldId);
        return Ok(result);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteBusinessField([FromQuery] Guid businessFieldId)
    {
        var result = await _businessFieldService.DeleteBusinessFieldById(businessFieldId);
        return Ok(result);
    }
}
}
