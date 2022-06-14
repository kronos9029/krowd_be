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
    [Route("api/v1.0/packages")]
    [EnableCors]
    //[Authorize]
    public class PackageController : ControllerBase
    {
        private readonly IPackageService _packageService;
        private readonly IHttpContextAccessor httpContextAccessor;
        public PackageController(IPackageService packageService, IHttpContextAccessor httpContextAccessor)
        {
            _packageService = packageService;
            this.httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePackage([FromBody] PackageDTO packageDTO)
        {
            var result = await _packageService.CreatePackage(packageDTO);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPackages(int pageIndex, int pageSize)
        {
            var result = new List<PackageDTO>();
            result = await _packageService.GetAllPackages(pageIndex, pageSize);
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetPackageById(Guid id)
        {
            PackageDTO dto = new PackageDTO();
            dto = await _packageService.GetPackageById(id);
            return Ok(dto);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdatePackage([FromBody] PackageDTO packageDTO, Guid id)
        {
            var result = await _packageService.UpdatePackage(packageDTO, id);
            return Ok(result);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeletePackage(Guid id)
        {
            var result = await _packageService.DeletePackageById(id);
            return Ok(result);
        }
    }
}
