using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RevenueSharingInvest.API.Extensions;
using RevenueSharingInvest.Business.Models.Constant;
using RevenueSharingInvest.Business.Services;
using RevenueSharingInvest.Business.Services.Impls;
using RevenueSharingInvest.Data.Models.Constants;
using RevenueSharingInvest.Data.Models.Constants.Enum;
using RevenueSharingInvest.Data.Models.DTOs;
using RevenueSharingInvest.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RevenueSharingInvest.API.Controllers
{
    [ApiController]
    [Route("api/v1.0/wallet_transactions")]
    [EnableCors]
    [Authorize]
    public class WalletTransactionController : ControllerBase
    {
        private readonly IWalletTransactionService _walletTransactionService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRoleService _roleService;
        private readonly IUserService _userService;
        public WalletTransactionController(IWalletTransactionService walletTransactionService,
            IHttpContextAccessor httpContextAccessor,
            IRoleService roleService,
            IUserService userService)
        {
            _walletTransactionService = walletTransactionService;
            _httpContextAccessor = httpContextAccessor;
            _roleService = roleService;
            _userService = userService;
        }

        /*        [HttpPost]
                public async Task<IActionResult> CreateWalletTransaction([FromBody] WalletTransactionDTO walletTransactionDTO)
                {
                    var result = await _walletTransactionService.CreateWalletTransaction(walletTransactionDTO);
                    return Ok(result);
                }

                [HttpPost]
                [Route("I1ToI2")]
                public async Task<IActionResult> TransferFromI1ToI2([FromForm] double amount)
                {
                    ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);

                    var result = await _walletTransactionService.TransferFromI1ToI2(currentUser, amount);
                    return Ok(result);
                }
        */
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllWalletTransactions(int pageIndex, int pageSize, Guid? userId, Guid? walletId, string fromDate, string toDate, string type, string order)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);

            if (currentUser.roleId.Equals(currentUser.adminRoleId)
                || currentUser.roleId.Equals(currentUser.projectManagerRoleId)
                || currentUser.roleId.Equals(currentUser.investorRoleId))
            {
                var result = await _walletTransactionService.GetAllWalletTransactions(pageIndex, pageSize, userId, walletId, fromDate, toDate, type, order, currentUser);
                return Ok(result);
            }
            return StatusCode((int)HttpStatusCode.Forbidden, "Only user with role ADMIN or PROJECT_MANAGER or INVESTOR can perform this action!!!");
        }

        //[HttpGet]
        //[Route("{id}")]
        //[Authorize]
        //public async Task<IActionResult> GetWalletTransactionById(Guid id)
        //{
        //    WalletTransactionDTO dto = new WalletTransactionDTO();
        //    dto = await _walletTransactionService.GetWalletTransactionById(id);
        //    return Ok(dto);
        //}

/*        [HttpPut]
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
        }*/

    }
}
