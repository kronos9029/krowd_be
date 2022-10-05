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
    [Route("api/v1.0/vouchers")]
    [EnableCors]
    //[Authorize]
    public class VoucherController : ControllerBase
    {
        private readonly IVoucherService _voucherService;
        private readonly IHttpContextAccessor httpContextAccessor;
        public VoucherController(IVoucherService voucherService, IHttpContextAccessor httpContextAccessor)
        {
            _voucherService = voucherService;
            this.httpContextAccessor = httpContextAccessor;
        }

        //CREATE
        [HttpPost]
        public async Task<IActionResult> CreateVoucher([FromBody] VoucherDTO voucherDTO)
        {
            var result = await _voucherService.CreateVoucher(voucherDTO);
            return Ok(result);
        }

        //GET ALL
        [HttpGet]
        public async Task<IActionResult> GetAllVouchers(int pageIndex, int pageSize)
        {
            var result = new List<VoucherDTO>();
            result = await _voucherService.GetAllVouchers(pageIndex, pageSize);
            return Ok(result);
        }

        //GET BY ID
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetVoucherById(Guid id)
        {
            VoucherDTO dto = new VoucherDTO();
            dto = await _voucherService.GetVoucherById(id);
            return Ok(dto);
        }

        //UPDATE
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateVoucher([FromBody] VoucherDTO voucherDTO, Guid id)
        {
            var result = await _voucherService.UpdateVoucher(voucherDTO, id);
            return Ok(result);
        }

        //DELETE
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteVoucher(Guid id)
        {
            var result = await _voucherService.DeleteVoucherById(id);
            return Ok(result);
        }
    }
}
