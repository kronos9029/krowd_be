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
    [Route("api/v1.0/wallet_transactions")]
    [EnableCors]
    //[Authorize]
    public class WalletTransactionController : ControllerBase
    {
        private readonly IWalletTransactionService _walletTransactionService;
        private readonly IHttpContextAccessor httpContextAccessor;
        public WalletTransactionController(IWalletTransactionService walletTransactionService, IHttpContextAccessor httpContextAccessor)
        {
            _walletTransactionService = walletTransactionService;
            this.httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        public async Task<IActionResult> CreateWalletTransaction([FromBody] WalletTransactionDTO walletTransactionDTO)
        {
            var result = await _walletTransactionService.CreateWalletTransaction(walletTransactionDTO);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllWalletTransactions(int pageIndex, int pageSize)
        {
            var result = new List<WalletTransactionDTO>();
            result = await _walletTransactionService.GetAllWalletTransactions(pageIndex, pageSize);
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetWalletTransactionById(Guid id)
        {
            WalletTransactionDTO dto = new WalletTransactionDTO();
            dto = await _walletTransactionService.GetWalletTransactionById(id);
            return Ok(dto);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateWalletTransaction([FromBody] WalletTransactionDTO walletTransactionDTO, Guid id)
        {
            var result = await _walletTransactionService.UpdateWalletTransaction(walletTransactionDTO, id);
            return Ok(result);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteWalletTransaction(Guid id)
        {
            var result = await _walletTransactionService.DeleteWalletTransactionById(id);
            return Ok(result);
        }
    }
}
