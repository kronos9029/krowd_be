using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RevenueSharingInvest.API.Extensions;
using RevenueSharingInvest.Business.Models.Constant;
using RevenueSharingInvest.Business.Services;
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
    [Route("api/v1.0/wallets")]
    [EnableCors]
    //[Authorize]
    public class WalletController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IProjectWalletService _projectWalletService;
        private readonly IInvestorWalletService _investorWalletService;
        private readonly IHttpContextAccessor httpContextAccessor;
        public WalletController(IInvestorWalletService investorWalletService, IProjectWalletService projectWalletService, IUserService userService, IRoleService roleService, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _roleService = roleService;
            _investorWalletService = investorWalletService;
            _projectWalletService = projectWalletService;
            this.httpContextAccessor = httpContextAccessor;
        }

        //GET ALL
        [HttpGet]
        public async Task<IActionResult> GetAllWallets()
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);

            if (currentUser.roleId.Equals(currentUser.investorRoleId))
            {
                var result = await _investorWalletService.GetAllInvestorWallets(currentUser);
                return Ok(result);
            }

            else if (currentUser.roleId.Equals(currentUser.projectManagerRoleId))
            {
                var result = await _projectWalletService.GetAllProjectWallets(currentUser);
                return Ok(result);
            }

            return StatusCode((int)HttpStatusCode.Forbidden, "Only user with role INVESTOR or PROJECT_MANAGER can perform this action!!!");
        }

        //GET BY ID
        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetInvestorWalletById(Guid id)
        {
            ThisUserObj currentUser = await GetCurrentUserInfo.GetThisUserInfo(HttpContext, _roleService, _userService);

            if (currentUser.roleId.Equals(currentUser.investorRoleId))
            {
                var result = await _investorWalletService.GetInvestorWalletById(id, currentUser);
                return Ok(result);
            }

            else if (currentUser.roleId.Equals(currentUser.projectManagerRoleId))
            {
                var result = await _projectWalletService.GetAllProjectWalletById(id, currentUser);
                return Ok(result);
            }

            return StatusCode((int)HttpStatusCode.Forbidden, "Only user with role INVESTOR or PROJECT_MANAGER can perform this action!!!");
        }
    }
}
