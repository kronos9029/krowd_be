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
    [Route("api/v1.0/packageVouchers")]
    [EnableCors]
    //[Authorize]
    public class PackageVoucherController : ControllerBase
    {
        private readonly IPackageVoucherService _packageVoucherService;
        private readonly IHttpContextAccessor httpContextAccessor;
        public PackageVoucherController(IPackageVoucherService packageVoucherService, IHttpContextAccessor httpContextAccessor)
        {
            _packageVoucherService = packageVoucherService;
            this.httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePackageVoucher([FromBody] PackageVoucherDTO packageVoucherDTO)
        {
            var result = await _packageVoucherService.CreatePackageVoucher(packageVoucherDTO);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPackageVouchers()
        {
            var result = new List<PackageVoucherDTO>();
            result = await _packageVoucherService.GetAllPackageVouchers();
            return Ok(result);
        }

        [HttpGet]
        [Route("{packageId},{voucherId}")]
        public async Task<IActionResult> GetPackageVoucherById(Guid packageId, Guid voucherId)
        {
            PackageVoucherDTO dto = new PackageVoucherDTO();
            dto = await _packageVoucherService.GetPackageVoucherById(packageId, voucherId);
            return Ok(dto);
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePackageVoucher([FromBody] PackageVoucherDTO packageVoucherDTO, [FromQuery] Guid packageId, [FromQuery] Guid voucherId)
        {
            var result = await _packageVoucherService.UpdatePackageVoucher(packageVoucherDTO, packageId, voucherId);
            return Ok(result);
        }

        [HttpDelete]
        [Route("{packageId},{voucherId}")]
        public async Task<IActionResult> DeletePackageVoucher(Guid packageId, Guid voucherId)
        {
            var result = await _packageVoucherService.DeletePackageVoucherById(packageId, voucherId);
            return Ok(result);
        }
    }
}
