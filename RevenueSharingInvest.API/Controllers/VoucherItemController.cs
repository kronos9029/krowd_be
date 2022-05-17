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
    [Route("api/v1.0/voucherItems")]
    [EnableCors]
    //[Authorize]
    public class VoucherItemController : ControllerBase
    {
        private readonly IVoucherItemService _voucherItemService;
        private readonly IHttpContextAccessor httpContextAccessor;
        public VoucherItemController(IVoucherItemService voucherItemService, IHttpContextAccessor httpContextAccessor)
        {
            _voucherItemService = voucherItemService;
            this.httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        public async Task<IActionResult> CreateVoucherItem([FromBody] VoucherItemDTO voucherItemDTO)
        {
            var result = await _voucherItemService.CreateVoucherItem(voucherItemDTO);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllVoucherItems()
        {
            var result = new List<VoucherItemDTO>();
            result = await _voucherItemService.GetAllVoucherItems();
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetVoucherItemById(Guid id)
        {
            VoucherItemDTO dto = new VoucherItemDTO();
            dto = await _voucherItemService.GetVoucherItemById(id);
            return Ok(dto);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateVoucherItem([FromBody] VoucherItemDTO voucherItemDTO, [FromQuery] Guid voucherItemId)
        {
            var result = await _voucherItemService.UpdateVoucherItem(voucherItemDTO, voucherItemId);
            return Ok(result);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteVoucherItem(Guid id)
        {
            var result = await _voucherItemService.DeleteVoucherItemById(id);
            return Ok(result);
        }
    }
}
