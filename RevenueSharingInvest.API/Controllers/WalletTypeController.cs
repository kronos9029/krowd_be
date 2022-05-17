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
    [Route("api/v1.0/walletTypes")]
    [EnableCors]
    //[Authorize]
    public class WalletTypeController : ControllerBase
    {
        private readonly IWalletTypeService _walletTypeService;
        private readonly IHttpContextAccessor httpContextAccessor;
        public WalletTypeController(IWalletTypeService walletTypeService, IHttpContextAccessor httpContextAccessor)
        {
            _walletTypeService = walletTypeService;
            this.httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        public async Task<IActionResult> CreateWalletType([FromBody] WalletTypeDTO walletTypeDTO)
        {
            var result = await _walletTypeService.CreateWalletType(walletTypeDTO);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllWalletTypes()
        {
            var result = new List<WalletTypeDTO>();
            result = await _walletTypeService.GetAllWalletTypes();
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetWalletTypeById(Guid id)
        {
            WalletTypeDTO dto = new WalletTypeDTO();
            dto = await _walletTypeService.GetWalletTypeById(id);
            return Ok(dto);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateWalletType([FromBody] WalletTypeDTO walletTypeDTO, [FromQuery] Guid walletTypeId)
        {
            var result = await _walletTypeService.UpdateWalletType(walletTypeDTO, walletTypeId);
            return Ok(result);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteWalletType(Guid id)
        {
            var result = await _walletTypeService.DeleteWalletTypeById(id);
            return Ok(result);
        }
    }
}
