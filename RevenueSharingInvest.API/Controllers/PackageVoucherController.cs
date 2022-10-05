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
    [Route("api/v1.0/package_vouchers")]
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

        //CREATE
        [HttpPost]
        public async Task<IActionResult> CreatePackageVoucher([FromBody] PackageVoucherDTO packageVoucherDTO)
        {
            var result = await _packageVoucherService.CreatePackageVoucher(packageVoucherDTO);
            return Ok(result);
        }

        //GET ALL
        [HttpGet]
        public async Task<IActionResult> GetAllPackageVouchers(int pageIndex, int pageSize)
        {
            var result = new List<PackageVoucherDTO>();
            result = await _packageVoucherService.GetAllPackageVouchers(pageIndex, pageSize);
            return Ok(result);
        }

        //GET BY ID
        [HttpGet]
        [Route("{package_id},{voucher_id}")]
        public async Task<IActionResult> GetPackageVoucherById(Guid package_id, Guid voucher_id)
        {
            PackageVoucherDTO dto = new PackageVoucherDTO();
            dto = await _packageVoucherService.GetPackageVoucherById(package_id, voucher_id);
            return Ok(dto);
        }

        //UPDATE
        [HttpPut]
        [Route("{package_id},{voucher_id}")]
        public async Task<IActionResult> UpdatePackageVoucher([FromBody] PackageVoucherDTO packageVoucherDTO, Guid package_id, Guid voucher_id)
        {
            var result = await _packageVoucherService.UpdatePackageVoucher(packageVoucherDTO, package_id, voucher_id);
            return Ok(result);
        }

        //DELETE
        [HttpDelete]
        [Route("{package_id},{voucher_id}")]
        public async Task<IActionResult> DeletePackageVoucher(Guid package_id, Guid voucher_id)
        {
            var result = await _packageVoucherService.DeletePackageVoucherById(package_id, voucher_id);
            return Ok(result);
        }
    }
}
