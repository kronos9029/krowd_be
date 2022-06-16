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
    [Route("api/v1.0/system_wallets")]
    [EnableCors]
    //[Authorize]
    public class SystemWalletController : ControllerBase
    {
        private readonly ISystemWalletService _systemWalletService;
        private readonly IHttpContextAccessor httpContextAccessor;
        public SystemWalletController(ISystemWalletService systemWalletService, IHttpContextAccessor httpContextAccessor)
        {
            _systemWalletService = systemWalletService;
            this.httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSystemWallet([FromBody] SystemWalletDTO systemWalletDTO)
        {
            var result = await _systemWalletService.CreateSystemWallet(systemWalletDTO);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSystemWallets(int pageIndex, int pageSize)
        {
            var result = new List<SystemWalletDTO>();
            result = await _systemWalletService.GetAllSystemWallets(pageIndex, pageSize);
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetSystemWalletById(Guid id)
        {
            SystemWalletDTO dto = new SystemWalletDTO();
            dto = await _systemWalletService.GetSystemWalletById(id);
            return Ok(dto);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateSystemWallet([FromBody] SystemWalletDTO systemWalletDTO, Guid id)
        {
            var result = await _systemWalletService.UpdateSystemWallet(systemWalletDTO, id);
            return Ok(result);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteSystemWallet(Guid id)
        {
            var result = await _systemWalletService.DeleteSystemWalletById(id);
            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> ClearAllSystemWalletData()
        {
            var result = await _systemWalletService.ClearAllSystemWalletData();
            return Ok(result);
        }
    }
}
