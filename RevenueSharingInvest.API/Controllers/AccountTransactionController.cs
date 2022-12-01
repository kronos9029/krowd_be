using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RevenueSharingInvest.API.Extensions;
using RevenueSharingInvest.Business.Models.Constant;
using RevenueSharingInvest.Business.Services;
using RevenueSharingInvest.Business.Services.Impls;
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
    [Route("api/v1.0/account_transactions")]
    [EnableCors]
    [Authorize]
    public class AccountTransactionController : ControllerBase
    {
        private readonly IAccountTransactionService _accountTransactionService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthenticateService _authenticateService;
        private readonly IRoleService _roleService;
        private readonly IUserService _userService;
        public AccountTransactionController(IAccountTransactionService accountTransactionService,
            IHttpContextAccessor httpContextAccessor,
            IAuthenticateService authenticateService,
            IRoleService roleService,
            IUserService userService)
        {
            _accountTransactionService = accountTransactionService;
            _httpContextAccessor = httpContextAccessor;
            _authenticateService = authenticateService;
            _roleService = roleService;
            _userService = userService;
        }


        //GET ALL
        [HttpGet]
        [Authorize(Roles ="ADMIN, INVESTOR, PROJECT_MANAGER")]
        public async Task<IActionResult> GetAllAccountTransactions(int pageIndex, int pageSize, string fromDate, string toDate, string sort)
        {

            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);
            if(fromDate == null || toDate == null || fromDate.Equals("") || toDate.Equals(""))
            {
                var result = await _accountTransactionService.GetAllAccountTransactions(pageIndex, pageSize, sort, currentUser);
                return Ok(result);
            } else
            {
                var result = await _accountTransactionService.GetAccountTransactionsByDate(pageIndex, pageSize, fromDate, toDate, sort, currentUser);
                return Ok(result);
            }
        }

/*        private async Task<ThisUserObj> GetThisUserInfo(HttpContext? httpContext)
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
                if (userDTO.business != null)
                {
                    currentUser.roleId = userDTO.role.id;
                    currentUser.businessId = userDTO.business.id;
                }
                else
                {
                    currentUser.roleId = userDTO.role.id;
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
        }*/



    }
}
