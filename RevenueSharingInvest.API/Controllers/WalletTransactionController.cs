using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RevenueSharingInvest.Business.Models.Constant;
using RevenueSharingInvest.Business.Services;
using RevenueSharingInvest.Business.Services.Impls;
using RevenueSharingInvest.Data.Models.DTOs;
using RevenueSharingInvest.Data.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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

        [HttpPost]
        public async Task<IActionResult> CreateWalletTransaction([FromBody] WalletTransactionDTO walletTransactionDTO)
        {
            var result = await _walletTransactionService.CreateWalletTransaction(walletTransactionDTO);
            return Ok(result);
        }

        [HttpPost]
        [Route("I1ToI2")]
        public async Task<IActionResult> TransferFromI1ToI2([FromForm] double amount)
        {
            ThisUserObj currentUser = await GetThisUserInfo(HttpContext);

            var result = await _walletTransactionService.TransferFromI1ToI2(currentUser, amount);
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


        private async Task<ThisUserObj> GetThisUserInfo(HttpContext? httpContext)
        {
            ThisUserObj currentUser = new();

            var checkUser = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.SerialNumber);
            if (checkUser == null)
            {
                currentUser.userId = "";
                currentUser.email = "";
                currentUser.investorId = "";
            }
            else
            {
                currentUser.userId = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.SerialNumber).Value;
                currentUser.email = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value;
                currentUser.investorId = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.GroupSid).Value;
            }

            List<RoleDTO> roleList = await _roleService.GetAllRoles();
            GetUserDTO? userDTO = await _userService.GetUserByEmail(currentUser.email);
            if (userDTO == null)
            {
                currentUser.roleId = "";
                currentUser.businessId = "";

            }
            else
            {
                currentUser.roleId = userDTO.role.id;
                if (userDTO.business != null)
                {
                    currentUser.businessId = userDTO.business.id;
                }
                else
                {
                    currentUser.businessId = "";
                }

            }
            foreach (RoleDTO role in roleList)
            {
                if (role.name.Equals(Enum.GetNames(typeof(RoleEnum)).ElementAt(0)))
                {
                    currentUser.adminRoleId = role.id;
                }
                if (role.name.Equals(Enum.GetNames(typeof(RoleEnum)).ElementAt(3)))
                {
                    currentUser.investorRoleId = role.id;
                }
                if (role.name.Equals(Enum.GetNames(typeof(RoleEnum)).ElementAt(1)))
                {
                    currentUser.businessManagerRoleId = role.id;
                }
                if (role.name.Equals(Enum.GetNames(typeof(RoleEnum)).ElementAt(2)))
                {
                    currentUser.projectManagerRoleId = role.id;
                }
            }

            return currentUser;

        }
    }
}
