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
    [Route("api/v1.0/wallet_types")]
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

        //GET ALL
        [HttpGet]
        public async Task<IActionResult> GetAllWalletTypes()
        {
            var result = new List<GetWalletTypeDTO>();
            result = await _walletTypeService.GetAllWalletTypes();
            return Ok(result);
        }

        //GET BY ID
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetWalletTypeById(Guid id)
        {
            GetWalletTypeDTO dto = new GetWalletTypeDTO();
            dto = await _walletTypeService.GetWalletTypeById(id);
            return Ok(dto);
        }
    }
}
