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
    [Route("api/v1.0/account_transactions")]
    [EnableCors]
    //[Authorize]
    public class AccountTransactionController : ControllerBase
    {
        private readonly IAccountTransactionService _accountTransactionService;
        private readonly IHttpContextAccessor httpContextAccessor;
        public AccountTransactionController(IAccountTransactionService accountTransactionService, IHttpContextAccessor httpContextAccessor)
        {
            _accountTransactionService = accountTransactionService;
            this.httpContextAccessor = httpContextAccessor;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAccountTransaction([FromBody] AccountTransactionDTO accountTransactionDTO)
        {
            var result = await _accountTransactionService.CreateAccountTransaction(accountTransactionDTO);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAccountTransactions(int pageIndex, int pageSize)
        {
            var result = new List<AccountTransactionDTO>();
            result = await _accountTransactionService.GetAllAccountTransactions(pageIndex, pageSize);
            return Ok(result);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetAccountTransactionById(Guid id)
        {
            AccountTransactionDTO dto = new AccountTransactionDTO();
            dto = await _accountTransactionService.GetAccountTransactionById(id);
            return Ok(dto);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateAccountTransaction([FromBody] AccountTransactionDTO accountTransactionDTO, Guid id)
        {
            var result = await _accountTransactionService.UpdateAccountTransaction(accountTransactionDTO, id);
            return Ok(result);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteAccountTransaction(Guid id)
        {
            var result = await _accountTransactionService.DeleteAccountTransactionById(id);
            return Ok(result);
        }
    }
}
