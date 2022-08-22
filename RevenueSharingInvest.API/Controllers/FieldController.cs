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
    [Route("api/v1.0/fields")]
    [EnableCors]
    [Authorize]
    public class FieldController : ControllerBase
    {
        private readonly IFieldService _fieldService;
        private readonly IHttpContextAccessor httpContextAccessor;
        public FieldController(IFieldService fieldService, IHttpContextAccessor httpContextAccessor)
        {
            _fieldService = fieldService;
            this.httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> CreateField([FromBody] FieldDTO fieldDTO)
        {
            var result = await _fieldService.CreateField(fieldDTO);
            return Ok(result);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetAllFields(int pageIndex, int pageSize)
        {
            
            var result = await _fieldService.GetAllFields(pageIndex, pageSize);
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetFieldById(Guid id)
        {
            
            var dto = await _fieldService.GetFieldById(id);
            return Ok(dto);
        }

        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> UpdateField([FromBody] FieldDTO fieldDTO, Guid id)
        {

            var result = await _fieldService.UpdateField(fieldDTO, id);
            return Ok(result);
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteField(Guid id)
        {
            var result = await _fieldService.DeleteFieldById(id);
            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> ClearAllFieldData()
        {
            var result = await _fieldService.ClearAllFieldData();
            return Ok(result);
        }
    }
}
